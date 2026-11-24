using System.Runtime.InteropServices;
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
/// Native PAM consumer. The operating-system PAM stack decides whether a password
/// and the account are valid. Only a single hidden password prompt is supported.
/// Must be validated on real Ubuntu; this code does not store Linux passwords.
/// </summary>
public sealed class PamAuthenticator
{
    private const int PamSuccess = 0;
    private const int PamPromptEchoOff = 1;
    private const int PamSilent = 0x8000;

    [StructLayout(LayoutKind.Sequential)]
    private struct PamMessage { public int Style; public IntPtr Msg; }
    [StructLayout(LayoutKind.Sequential)]
    private struct PamResponse { public IntPtr Resp; public int RetCode; }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int ConversationFn(int messageCount, IntPtr messages, out IntPtr responses, IntPtr userData);
    [StructLayout(LayoutKind.Sequential)]
    private struct Conversation { public ConversationFn Fn; public IntPtr UserData; }

    [DllImport("libpam.so.0", EntryPoint = "pam_start", CallingConvention = CallingConvention.Cdecl)]
    private static extern int PamStart(string service, string user, ref Conversation conversation, out IntPtr handle);
    [DllImport("libpam.so.0", EntryPoint = "pam_authenticate", CallingConvention = CallingConvention.Cdecl)]
    private static extern int PamAuthenticate(IntPtr handle, int flags);
    [DllImport("libpam.so.0", EntryPoint = "pam_acct_mgmt", CallingConvention = CallingConvention.Cdecl)]
    private static extern int PamAccount(IntPtr handle, int flags);
    [DllImport("libpam.so.0", EntryPoint = "pam_end", CallingConvention = CallingConvention.Cdecl)]
    private static extern int PamEnd(IntPtr handle, int status);
    [DllImport("libc", EntryPoint = "calloc", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr Calloc(nuint count, nuint size);
    [DllImport("libc", EntryPoint = "strdup", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr StrDup([MarshalAs(UnmanagedType.LPUTF8Str)] string value);
    [DllImport("libc", EntryPoint = "free", CallingConvention = CallingConvention.Cdecl)]
    private static extern void Free(IntPtr pointer);

    public bool Authenticate(string user, string password)
    {
        if (!OperatingSystem.IsLinux() || !LinuxUserNames.IsValid(user) || password.Contains('\0')) return false;
        ConversationFn callback = (int count, IntPtr msgPtrs, out IntPtr responsePtr, IntPtr _) =>
        {
            responsePtr = IntPtr.Zero;
            if (count <= 0 || count > 8) return 19; // PAM_CONV_ERR
            var size = Marshal.SizeOf<PamResponse>();
            var buffer = Calloc((nuint)count, (nuint)size);
            if (buffer == IntPtr.Zero) return 5; // PAM_BUF_ERR
            for (var i = 0; i < count; i++)
            {
                var messageAddress = Marshal.ReadIntPtr(msgPtrs, i * IntPtr.Size);
                if (messageAddress == IntPtr.Zero) { Cleanup(buffer, i, size); return 19; }
                var msg = Marshal.PtrToStructure<PamMessage>(messageAddress);
                if (msg.Style != PamPromptEchoOff) { Cleanup(buffer, i, size); return 19; }
                var pw = StrDup(password);
                if (pw == IntPtr.Zero) { Cleanup(buffer, i, size); return 5; }
                Marshal.StructureToPtr(new PamResponse { Resp = pw, RetCode = 0 }, IntPtr.Add(buffer, i * size), false);
            }
            responsePtr = buffer;
            return PamSuccess;
        };

        var conv = new Conversation { Fn = callback, UserData = IntPtr.Zero };
        IntPtr pam = IntPtr.Zero;
        var status = 19;
        try
        {
            status = PamStart("wallforge", user, ref conv, out pam);
            if (status != PamSuccess || pam == IntPtr.Zero) return false;
            status = PamAuthenticate(pam, PamSilent);
            if (status != PamSuccess) return false;
            status = PamAccount(pam, PamSilent);
            return status == PamSuccess;
        }
        finally
        {
            if (pam != IntPtr.Zero) PamEnd(pam, status);
            GC.KeepAlive(callback);
        }
    }

    private static void Cleanup(IntPtr buffer, int initialized, int stride)
    {
        for (var index = 0; index < initialized; index++)
        {
            var entry = Marshal.PtrToStructure<PamResponse>(IntPtr.Add(buffer, index * stride));
            if (entry.Resp != IntPtr.Zero) Free(entry.Resp);
        }
        Free(buffer);
    }
}
