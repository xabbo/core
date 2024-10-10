using System;
using System.Collections.Generic;
using System.Linq;

namespace Xabbo.Core.GameData;

public sealed class GameDataHashes : Dictionary<GameDataType, string>, IEquatable<GameDataHashes>
{
    public override int GetHashCode() => this
        .OrderBy(x => x.Key)
        .Aggregate(
            Count.GetHashCode(),
            (hashCode, entry) => HashCode.Combine(hashCode, entry.Value));

    public override bool Equals(object? obj) => obj is GameDataHashes other && Equals(other);

    public bool Equals(GameDataHashes? other)
    {
        if (other is null)
            return false;

        if (Count != other.Count)
            return false;

        foreach (var (key, value) in this)
        {
            if (!other.TryGetValue(key, out var otherValue) ||
                !string.Equals(value, otherValue))
            {
                return false;
            }
        }

        return true;
    }
}