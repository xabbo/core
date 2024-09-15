using System;
using System.Runtime.CompilerServices;

using Microsoft.Extensions.Logging;

namespace Xabbo.Core;

internal static class LogExtensions
{
    public static IDisposable? MethodScope(this ILogger logger, [CallerMemberName] string? name = null)
        => logger.BeginScope("[{Method}]", name);
}
