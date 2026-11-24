using System.Buffers.Binary;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Antiforgery;

namespace WallForge.Api;

public static class LinuxUserNames
{
    private static readonly Regex Pattern = new(@"^[a-z_][a-z0-9_-]{0,31}\$?$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    public static bool IsValid(string? name) => name is not null && name != "root" && Pattern.IsMatch(name);
}

public static class CsrfGuard
{
    public static async Task<bool> ValidAsync(HttpContext context, IAntiforgery antiforgery)
    {
        // Strict same-site cookies and explicit anti-forgery token are both mandatory.
        if (context.Request.Headers.Origin.Count > 0 &&
            !string.Equals(context.Request.Headers.Origin.ToString(),
                $"{context.Request.Scheme}://{context.Request.Host}", StringComparison.OrdinalIgnoreCase))
            return false;
        try { return await antiforgery.IsRequestValidAsync(context); }
        catch (AntiforgeryValidationException) { return false; }
    }
}

/// <summary>
/// An unprivileged .NET client for one fixed, argument-exact privileged PAM helper.
/// Secrets travel over the child's standard input, never command-line arguments.
/// </summary>
public sealed class PamAuthenticator
{
    private const int MaximumPasswordBytes = 1024;

    public async Task<bool> AuthenticateAsync(string username, string password, CancellationToken cancellationToken)
    {
        if (!OperatingSystem.IsLinux() || !LinuxUserNames.IsValid(username) ||
            string.IsNullOrEmpty(password) || password.Contains('\0')) return false;

        var nameBytes = Encoding.UTF8.GetBytes(username);
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        if (nameBytes.Length > 32 || passwordBytes.Length > MaximumPasswordBytes) return false;

        byte[]? payload = null;
        try
        {
            payload = new byte[4 + nameBytes.Length + passwordBytes.Length];
            BinaryPrimitives.WriteUInt16BigEndian(payload.AsSpan(0, 2), (ushort)nameBytes.Length);
            BinaryPrimitives.WriteUInt16BigEndian(payload.AsSpan(2, 2), (ushort)passwordBytes.Length);
            nameBytes.CopyTo(payload.AsSpan(4));
            passwordBytes.CopyTo(payload.AsSpan(4 + nameBytes.Length));

            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeout.CancelAfter(TimeSpan.FromSeconds(10));
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo("/usr/bin/sudo")
                {
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = false,
                    CreateNoWindow = true
                }
            };
            process.StartInfo.ArgumentList.Add("-n");
            process.StartInfo.ArgumentList.Add("/usr/local/libexec/wallforge-pam-auth");
            process.StartInfo.Environment["LC_ALL"] = "C";
            if (!process.Start()) return false;
            try
            {
                await process.StandardInput.BaseStream.WriteAsync(payload, timeout.Token);
                await process.StandardInput.BaseStream.FlushAsync(timeout.Token);
                process.StandardInput.Close();
                var answer = new byte[1];
                var count = await process.StandardOutput.BaseStream.ReadAsync(answer, timeout.Token);
                await process.WaitForExitAsync(timeout.Token);
                return count == 1 && answer[0] == (byte)'1' && process.ExitCode == 0;
            }
            finally
            {
                if (!process.HasExited)
                {
                    try { process.Kill(entireProcessTree: true); }
                    catch (InvalidOperationException) { }
                }
            }
        }
        catch (Exception ex) when (ex is IOException or System.ComponentModel.Win32Exception or InvalidOperationException or OperationCanceledException)
        {
            return false; // Fail closed when privilege broker is unavailable.
        }
        finally
        {
            CryptographicOperations.ZeroMemory(passwordBytes);
            if (payload is not null) CryptographicOperations.ZeroMemory(payload);
        }
    }
}
