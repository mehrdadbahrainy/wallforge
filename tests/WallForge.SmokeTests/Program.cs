using WallForge.Api;
var count = 0;
void Verify(bool condition, string message)
{
    if (!condition) throw new Exception("FAILED: " + message);
    Console.WriteLine("PASS: " + message);
    count++;
}
Verify(!LinuxUserNames.IsValid("root"), "root is denied");
Verify(!LinuxUserNames.IsValid("bad;command"), "command-like usernames rejected");
Verify(LinuxUserNames.IsValid("admin-user"), "regular username accepted");
Verify(LocalAccountStore.ParseUser("alex:x:1001:1001:Alex:/home/alex:/bin/bash")?.UserName == "alex", "parse local passwd entry");
Verify(LocalAccountStore.ParseUser("bad:x:notnum:1001::/:/bin/sh") is null, "reject invalid UID");
Verify(LocalAccountStore.ParseGroup("wallforge-admin:x:1988:alex")?.Members.SequenceEqual(["alex"]) == true, "parse local group entry");
Verify(LinuxUserNames.IsValid(new string('a', 31) + "$"), "32-byte machine username accepted");
Verify(!LinuxUserNames.IsValid(new string('a', 32) + "$"), "33-byte machine username rejected consistently with PAM helper");
Verify(!LinuxUserNames.IsValid("alex\n"), "username with trailing newline rejected");
var temp = Path.Combine(Path.GetTempPath(), "wf-test-" + Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(temp);
try
{
    var passwd = Path.Combine(temp, "passwd"); var group = Path.Combine(temp, "group");
    File.WriteAllText(passwd, "root:x:0:0:root:/root:/bin/bash\nalex:x:1001:1001::/home/alex:/bin/bash\nbob:x:1002:1002::/home/bob:/bin/bash\n");
    File.WriteAllText(group, "root:x:0:\nwallforge-admin:x:1988:alex\nwallforge-viewer:x:1989:bob\n");
    var store = new LocalAccountStore(passwd, group);
    Verify(store.GetRole("alex") == "admin", "admin membership grants admin");
    Verify(store.GetRole("bob") == "viewer", "viewer membership grants read-only");
    Verify(store.GetRole("root") is null, "root cannot become an application admin");
    Verify(store.GetRole("unlisted") is null, "unlisted user denied");
    File.WriteAllText(group, "wallforge-viewer:x:1989:bob\n");
    Verify(store.GetRole("alex") is null, "removed group membership revokes access immediately");
}
finally { Directory.Delete(temp, true); }
var ufw = UfwInspector.Parse("Status: active\n\n[ 1] 22/tcp ALLOW IN Anywhere\n[ 2] 80/tcp DENY IN Anywhere\n");
Verify(ufw.Available && ufw.Status == "active", "parse UFW active state");
Verify(ufw.Rules.Length == 2, "parse UFW numbered rules");
Verify(UfwInspector.Parse("garbage").Available == false, "bad UFW output fails closed");
Console.WriteLine($"{count} WallForge smoke tests passed.");
