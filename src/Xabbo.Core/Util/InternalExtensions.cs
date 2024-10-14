using System;
using System.Collections.Concurrent;

namespace Xabbo.Core;

internal static class InternalExtensions
{
    /// <summary>
    /// Adds or updates the specified item in the dictionary, and returns whether the item was added
    /// via the '<paramref name="added"/>' out parameter.
    /// Returns the previous value if the item was updated, or the default value if it was added.
    /// </summary>
    public static TValue? AddOrUpdate<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary,
        TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValue, out bool added) where TKey : notnull
    {
        while (true)
        {
            if (dictionary.TryAdd(key, addValue))
            {
                added = true;
                return default;
            }
            else if (dictionary.TryGetValue(key, out TValue? existingValue))
            {
                TValue newValue = updateValue(key, existingValue);
                if (dictionary.TryUpdate(key, newValue, existingValue))
                {
                    added = false;
                    return existingValue;
                }
            }
        }
    }
}
