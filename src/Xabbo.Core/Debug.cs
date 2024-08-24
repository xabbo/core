using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace Xabbo.Core;

internal static class Debug
{
    [Conditional("DEBUG")]
    public static void Log(string message,
        [CallerMemberName] string? memberName = null,
        [CallerFilePath] string? sourceFilePath = null,
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        string? fileName = Path.GetFileNameWithoutExtension(sourceFilePath);
        System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {fileName}.{memberName}: {message}");
    }
}
