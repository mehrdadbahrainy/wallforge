using System.Diagnostics;

namespace WallForge.Api;

public sealed record FirewallSnapshot(bool Available, string Status, string[] Rules, string? Error);

/// <summary>Execute only one fixed, read-only sudo command. No user-controlled args or shell.</summary>
public sealed class UfwInspector
{
    public async Task<FirewallSnapshot> GetStatusAsync(CancellationToken cancel)
    {
        if (!OperatingSystem.IsLinux())
            return new(false, "unsupported", [], "Linux is required.");
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancel);
        timeout.CancelAfter(TimeSpan.FromSeconds(8));
        try
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo("/usr/bin/sudo")
                {
                    UseShellExecute = false, RedirectStandardOutput = true,
                    RedirectStandardError = true, CreateNoWindow = true
                }
            };
            process.StartInfo.ArgumentList.Add("-n");
            process.StartInfo.ArgumentList.Add("/usr/sbin/ufw");
            process.StartInfo.ArgumentList.Add("status");
            process.StartInfo.ArgumentList.Add("numbered");
            process.StartInfo.Environment["LC_ALL"] = "C";
            if (!process.Start()) return new(false, "unavailable", [], "Unable to start UFW.");
            var stdout = process.StandardOutput.ReadToEndAsync(timeout.Token);
            var stderr = process.StandardError.ReadToEndAsync(timeout.Token);
            await Task.WhenAll(stdout, stderr, process.WaitForExitAsync(timeout.Token));
            if (process.ExitCode != 0)
                return new(false, "unavailable", [], "UFW status cannot be read. Check the documented sudoers permission.");
            return Parse(stdout.Result);
        }
        catch (OperationCanceledException) when (!cancel.IsCancellationRequested)
        { return new(false, "unavailable", [], "Firewall inspection timed out."); }
        catch (Exception e) when (e is IOException or System.ComponentModel.Win32Exception or InvalidOperationException)
        { return new(false, "unavailable", [], "The read-only firewall inspector is unavailable."); }
    }

    public static FirewallSnapshot Parse(string raw)
    {
        var lines = raw.Replace("\r", "", StringComparison.Ordinal).Split('\n');
        var header = lines.Select(x => x.Trim()).FirstOrDefault(x => x.StartsWith("Status:", StringComparison.OrdinalIgnoreCase));
        if (header is null) return new(false, "unknown", [], "UFW output could not be parsed.");
        var status = header[7..].Trim().ToLowerInvariant();
        if (status is not ("active" or "inactive")) return new(false, "unknown", [], "Unrecognized UFW state.");
        var rules = lines.Select(x => x.Trim()).Where(x => x.StartsWith('[', StringComparison.Ordinal) && x.Contains(']')).ToArray();
        return new(true, status, rules, null);
    }
}
