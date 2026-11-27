using System.Globalization;

namespace WallForge.Api;

public sealed record LinuxUser(string UserName, int Uid, int Gid, string Home, string Shell, bool SystemAccount);
public sealed record LinuxGroup(string Name, int Gid, string[] Members);
public sealed record UsersSnapshot(LinuxUser[] Users, LinuxGroup[] Groups);

/// <summary>Ubuntu local /etc/passwd and /etc/group only. Never reads shadow or passwords.</summary>
public sealed class LocalAccountStore
{
    private readonly string _passwd;
    private readonly string _group;
    public LocalAccountStore() : this("/etc/passwd", "/etc/group") { }
    public LocalAccountStore(string passwdPath, string groupPath)
    { _passwd = passwdPath; _group = groupPath; }

    public UsersSnapshot GetInventory()
    {
        var users = File.ReadLines(_passwd).Select(ParseUser).Where(u => u is not null).Cast<LinuxUser>().ToArray();
        var groups = File.ReadLines(_group).Select(ParseGroup).Where(g => g is not null).Cast<LinuxGroup>().ToArray();
        return new(users, groups);
    }

    public string? GetRole(string? userName)
    {
        if (!LinuxUserNames.IsValid(userName)) return null;
        try
        {
            var data = GetInventory();
            var user = data.Users.FirstOrDefault(u => string.Equals(u.UserName, userName, StringComparison.Ordinal));
            if (user is null || user.SystemAccount) return null;
            var groups = data.Groups.Where(g => g.Gid == user.Gid || g.Members.Contains(userName!, StringComparer.Ordinal)).Select(g => g.Name).ToHashSet(StringComparer.Ordinal);
            if (groups.Contains("wallforge-admin")) return "admin";
            if (groups.Contains("wallforge-viewer")) return "viewer";
            return null;
        }
        catch (IOException) { return null; }
        catch (UnauthorizedAccessException) { return null; }
    }

    public static LinuxUser? ParseUser(string line)
    {
        var p = line.Split(':');
        if (p.Length != 7 ||
            !int.TryParse(p[2], NumberStyles.None, CultureInfo.InvariantCulture, out var uid) ||
            !int.TryParse(p[3], NumberStyles.None, CultureInfo.InvariantCulture, out var gid))
            return null;
        return new LinuxUser(p[0], uid, gid, p[5], p[6], uid < 1000 || uid == 65534);
    }

    public static LinuxGroup? ParseGroup(string line)
    {
        var p = line.Split(':');
        if (p.Length != 4 || !int.TryParse(p[2], NumberStyles.None, CultureInfo.InvariantCulture, out var gid))
            return null;
        return new LinuxGroup(p[0], gid, p[3].Length == 0 ? [] : p[3].Split(',', StringSplitOptions.RemoveEmptyEntries));
    }
}
