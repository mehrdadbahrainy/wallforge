using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using System.Security.Cryptography.X509Certificates;
using WallForge.Api;
if (!OperatingSystem.IsLinux()) throw new InvalidOperationException("Linux integration tests must execute on Ubuntu.");
if (args is ["--pam-overlong"])
{
    if (await new PamAuthenticator().AuthenticateAsync("wfadmin", new string('é', 600), CancellationToken.None))
        throw new Exception("Oversized UTF8 password was accepted");
    Console.WriteLine("PASS password exceeding1024 UTF8 bytes is rejected before helper invocation");
    return;
}
if (args is ["--audit-contract"])
{
    var logger = new AuditCapture();
    var audit = new AuditStore(logger);
    for (var index = 0; index < 300; index++) audit.Record("fixture-" + index, "fixture-actor");
    var entries = audit.Recent();
    if (entries.Length != 250 || entries[0].Event != "fixture-299" || entries[^1].Event != "fixture-50")
        throw new Exception("Bounded audit ordering/retention changed");
    Console.WriteLine("PASS audit UI retains the newest250 events in reverse order");
    if (logger.Count != 300 || logger.LastLevel != Microsoft.Extensions.Logging.LogLevel.Information)
        throw new Exception("Evicted audit entries did not reach the configured logger");
    Console.WriteLine("PASS all300 audit events reach ILogger, including evicted UI events");
    if (new AuditStore(logger).Recent().Length != 0) throw new Exception("Unexpected durable UI audit storage");
    Console.WriteLine("PASS audit UI reset is explicit; journal durability is a separate host gate");
    return;
}
if (args is ["--ufw-cancel"])
{
    using var cancellation = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));
    try { await new UfwInspector().GetStatusAsync(cancellation.Token); throw new Exception("Cancellation was ignored"); }
    catch (OperationCanceledException) when (cancellation.IsCancellationRequested) { }
    await Task.Delay(TimeSpan.FromSeconds(8));
    var pid = File.ReadAllText("/tmp/wallforge-hanging-ufw.pid").Trim();
    if (Directory.Exists("/proc/" + pid)) throw new Exception("Cancelled request left an unbounded UFW child");
    Console.WriteLine("PASS request cancellation does not leave UFW alive beyond the privileged deadline");
    return;
}
if (args is ["--ufw-timeout"])
{
    var watch = System.Diagnostics.Stopwatch.StartNew();
    var snapshot = await new UfwInspector().GetStatusAsync(CancellationToken.None);
    if (snapshot.Available || watch.Elapsed > TimeSpan.FromSeconds(12)) throw new Exception("UFW hang was not bounded");
    var marker = "/tmp/wallforge-hanging-ufw.pid";
    if (!File.Exists(marker)) throw new Exception("Hanging fixture did not execute");
    var pid = File.ReadAllText(marker).Trim();
    if (Directory.Exists("/proc/" + pid)) throw new Exception("Timed-out UFW child survived the request");
    Console.WriteLine("PASS hanging UFW terminates within the privileged deadline; no child survives");
    return;
}
if (args is ["--ufw-unavailable"])
{
    var snapshot = await new UfwInspector().GetStatusAsync(CancellationToken.None);
    if (snapshot.Available || string.IsNullOrWhiteSpace(snapshot.Error)) throw new Exception("Missing UFW did not fail gracefully");
    Console.WriteLine("PASS unavailable UFW fails closed with a safe error");
    return;
}
if (args is ["--session-expiry", var baseUrl, var keys, var certificatePath, var username])
{
    var provider = DataProtectionProvider.Create(new DirectoryInfo(keys), b => b.SetApplicationName("WallForge"));
    var format = new TicketDataFormat(provider.CreateProtector("Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationMiddleware", "WallForgeCookie", "v2"));
    var certificate = X509CertificateLoader.LoadCertificateFromFile(certificatePath);
    using var handler = new HttpClientHandler { UseCookies = false };
    handler.ServerCertificateCustomValidationCallback = (_, cert, _, _) => cert is not null && cert.RawData.SequenceEqual(certificate.RawData);
    using var client = new HttpClient(handler) { BaseAddress = new Uri(baseUrl) };
    foreach (var expired in new[] { false, true })
    {
        var properties = new AuthenticationProperties
        {
            IssuedUtc = DateTimeOffset.UtcNow.AddMinutes(-30),
            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(expired ? -1 : 20)
        };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username) }, "WallForgeCookie"));
        var ticket = format.Protect(new AuthenticationTicket(principal, properties, "WallForgeCookie"));
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/auth/me");
        request.Headers.Add("Cookie", "__Host-wallforge-session=" + ticket);
        using var response = await client.SendAsync(request);
        if ((int)response.StatusCode != (expired ? 401 : 200)) throw new Exception("Unexpected ticket expiration result");
        Console.WriteLine(expired ? "PASS expired protected ticket returns 401" : "PASS unexpired protected ticket returns 200");
    }
    return;
}
if (args.Length < 3) throw new ArgumentException("Usage: <user> <role|none> <mode: valid|invalid-password|account-disabled|read-ufw>");
var user = args[0];
var expectedRole = args[1];
var mode = args[2];
var password = Console.ReadLine();
if (string.IsNullOrEmpty(password)) throw new Exception("Expected password on standard input.");

var accounts = new LocalAccountStore();
var role = accounts.GetRole(user);
if ((role ?? "none") != expectedRole)
    throw new Exception($"Unexpected role: {role ?? "none"} (expected {expectedRole})");
if (!accounts.GetInventory().Users.Any(x => x.UserName == user))
    throw new Exception("Local user not in inventory");

var auth = new PamAuthenticator();
if (mode == "valid" || mode == "read-ufw")
{
    if (!await auth.AuthenticateAsync(user, password, CancellationToken.None)) throw new Exception("PAM rejected valid local Ubuntu credentials");
}
else if (mode == "invalid-password" || mode == "account-disabled")
{
    if (await auth.AuthenticateAsync(user, password, CancellationToken.None)) throw new Exception("PAM accepted an invalid/disabled account");
}
else throw new ArgumentException("Unsupported test mode");

if (mode == "read-ufw")
{
    var firewall = await new UfwInspector().GetStatusAsync(CancellationToken.None);
    if (!firewall.Available)
        throw new Exception("UFW inspection failed: " + firewall.Error);
    Console.WriteLine("PASS read-only UFW status: " + firewall.Status);
}
Console.WriteLine("PASS Linux integration: " + mode + " / " + expectedRole);

sealed class AuditCapture : Microsoft.Extensions.Logging.ILogger<AuditStore>
{
    public int Count { get; private set; }
    public Microsoft.Extensions.Logging.LogLevel LastLevel { get; private set; }
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
    public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel level) => true;
    public void Log<TState>(Microsoft.Extensions.Logging.LogLevel level, Microsoft.Extensions.Logging.EventId id, TState state, Exception? error, Func<TState, Exception?, string> formatter)
    { Count++; LastLevel = level; }
}
