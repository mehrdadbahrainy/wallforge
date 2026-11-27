using System.Security.Claims;
using System.Net;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.DataProtection;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.RateLimiting;
using WallForge.Api;

var builder = WebApplication.CreateBuilder(args);
var keyDirectory = builder.Configuration["WALLFORGE_KEYS_DIR"];
if (!string.IsNullOrWhiteSpace(keyDirectory))
{
    Directory.CreateDirectory(keyDirectory);
    builder.Services.AddDataProtection()
        .SetApplicationName("WallForge")
        .PersistKeysToFileSystem(new DirectoryInfo(keyDirectory));
}
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownProxies.Add(IPAddress.Loopback);
    options.KnownProxies.Add(IPAddress.IPv6Loopback);
});
builder.Services.AddSingleton<LocalAccountStore>();
builder.Services.AddSingleton<PamAuthenticator>();
builder.Services.AddSingleton<UfwInspector>();
builder.Services.AddSingleton<AuditStore>();
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-WallForge-CSRF";
    options.Cookie.Name = "__Host-wallforge-xsrf";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.Path = "/";
});
builder.Services.AddAuthentication("WallForgeCookie").AddCookie("WallForgeCookie", options =>
{
    options.Cookie.Name = "__Host-wallforge-session";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.Path = "/";
    options.SlidingExpiration = false;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    options.Events.OnRedirectToLogin = context => { context.Response.StatusCode = 401; return Task.CompletedTask; };
    options.Events.OnRedirectToAccessDenied = context => { context.Response.StatusCode = 403; return Task.CompletedTask; };
});
builder.Services.AddAuthorization();
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("csrf", context => RateLimitPartition.GetFixedWindowLimiter(
        context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 30, Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0, AutoReplenishment = true
        }));
    options.AddPolicy("login", context => RateLimitPartition.GetFixedWindowLimiter(
        context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 5, Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0, AutoReplenishment = true
        }));
});
var app = builder.Build();
app.UseForwardedHeaders();
// Host inventory and authentication responses must not survive in browser/proxy caches.
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/api"))
        context.Response.Headers.CacheControl = "no-store";
    await next(context);
});
app.UseExceptionHandler(handler =>
{
    handler.Run(async context =>
    {
        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new { error = "An unexpected server error occurred." });
    });
});
app.UseHsts();
app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/api/health", () => Results.Ok(new { status = "ok", version = "0.1.0-rc.1" }));

app.MapGet("/api/auth/csrf", (HttpContext context, IAntiforgery antiforgery) =>
{
    if (!context.Request.IsHttps) return Results.StatusCode(StatusCodes.Status403Forbidden);
    var tokens = antiforgery.GetAndStoreTokens(context);
    context.Response.Headers.CacheControl = "no-store";
    return Results.Ok(new { token = tokens.RequestToken });
}).RequireRateLimiting("csrf");

app.MapPost("/api/auth/login", async (
    HttpContext context, LoginRequest input, IAntiforgery antiforgery,
    PamAuthenticator pam, LocalAccountStore accounts, AuditStore audit) =>
{
    if (!await CsrfGuard.ValidAsync(context, antiforgery))
        return Results.BadRequest(new { error = "Invalid request token." });
    if (!LinuxUserNames.IsValid(input.UserName) || string.IsNullOrEmpty(input.Password) || input.Password.Length > 1024)
        return Results.BadRequest(new { error = "Invalid credentials." });
    var role = accounts.GetRole(input.UserName);
    if (role is null || !await pam.AuthenticateAsync(input.UserName, input.Password, context.RequestAborted))
    {
        audit.Record("login-denied", "anonymous");
        return Results.Unauthorized();
    }
    var principal = new ClaimsPrincipal(new ClaimsIdentity(
        new[] { new Claim(ClaimTypes.Name, input.UserName) }, "WallForgeCookie"));
    await context.SignInAsync("WallForgeCookie", principal);
    audit.Record("login", input.UserName);
    context.Response.Headers.CacheControl = "no-store";
    return Results.Ok(new UserSession(input.UserName, role));
}).RequireRateLimiting("login");

var session = app.MapGroup("/api").RequireAuthorization();
session.AddEndpointFilter(async (invocation, next) =>
{
    var context = invocation.HttpContext;
    var accounts = context.RequestServices.GetRequiredService<LocalAccountStore>();
    var name = context.User.Identity?.Name;
    var role = accounts.GetRole(name);
    if (role is null)
    {
        await context.SignOutAsync("WallForgeCookie");
        return Results.Unauthorized();
    }
    context.Items["wallforge-role"] = role;
    return await next(invocation);
});

session.MapGet("/auth/me", (HttpContext context) =>
{
    context.Response.Headers.CacheControl = "no-store";
    return Results.Ok(new UserSession(context.User.Identity!.Name!, (string)context.Items["wallforge-role"]!));
});

session.MapPost("/auth/logout", async (HttpContext context, IAntiforgery antiforgery, AuditStore audit) =>
{
    if (!await CsrfGuard.ValidAsync(context, antiforgery))
        return Results.BadRequest(new { error = "Invalid request token." });
    audit.Record("logout", context.User.Identity!.Name!);
    await context.SignOutAsync("WallForgeCookie");
    return Results.NoContent();
});

session.MapGet("/host/firewall", async (HttpContext context, UfwInspector ufw, AuditStore audit, CancellationToken ct) =>
{
    audit.Record("read-firewall", context.User.Identity!.Name!);
    return Results.Ok(await ufw.GetStatusAsync(ct));
});

session.MapGet("/host/users", (HttpContext context, LocalAccountStore accounts, AuditStore audit) =>
{
    audit.Record("read-users", context.User.Identity!.Name!);
    return Results.Ok(accounts.GetInventory());
});

session.MapGet("/audit", (HttpContext context, AuditStore audit) =>
{
    if ((string)context.Items["wallforge-role"]! != "admin") return Results.Forbid();
    return Results.Ok(audit.Recent());
});

// Keep unknown API routes out of the Angular SPA fallback.
app.MapGet("/api/{**path}", () => Results.NotFound(new { error = "API endpoint not found." }));
app.MapFallbackToFile("index.html");
app.Run();

public sealed record LoginRequest(string UserName, string Password);
public sealed record UserSession(string UserName, string Role);
public partial class Program { }
