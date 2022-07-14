using System;
using System.Collections.Concurrent;

namespace Xabbo.Core;

internal static class InternalExtensions
{
    public static TValue AddOrUpdate<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary,
        TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValue, out bool added) where TKey : notnull
    {
        while (true)
        {
            if (dictionary.TryAdd(key, addValue))
            {
                added = true;
                return addValue;
            }
            else if (dictionary.TryGetValue(key, out TValue? existingValue))
            {
                TValue newValue = updateValue(key, existingValue);
                if (dictionary.TryUpdate(key, newValue, existingValue))
                {
                    added = false;
                    return newValue;
                }
            }
        }
    }
}
