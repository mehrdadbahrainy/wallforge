using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using System.Security.Cryptography.X509Certificates;
using WallForge.Api;
if (!OperatingSystem.IsLinux()) throw new InvalidOperationException("Linux integration tests must execute on Ubuntu.");
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
