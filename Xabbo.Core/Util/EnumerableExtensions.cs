using System;
using System.Collections.Generic;
using System.Linq;

namespace Xabbo.Core;

internal static class EnumerableExtensions
{
    public static IEnumerable<T> FindDuplicates<T>(this IEnumerable<T> enumerable)
    {
        return enumerable
            .GroupBy(x => x)
            .Where(x => x.Count() > 1)
            .Select(x => x.Key);
    }

    public static T? FindDuplicate<T>(this IEnumerable<T> enumerable) => FindDuplicates(enumerable).FirstOrDefault();
}
