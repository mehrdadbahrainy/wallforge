using WallForge.Api;
if (!OperatingSystem.IsLinux()) throw new InvalidOperationException("Linux integration tests must execute on Ubuntu.");
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
