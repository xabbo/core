using System;

namespace Xabbo.Core;

/// <summary>
/// Defines crackable state as extra data in an item.
/// </summary>
public interface ICrackableFurniData : IItemData
{
    /// <summary>
    /// The number of hits the crackable furni has taken.
    /// </summary>
    int Hits { get; }
    /// <summary>
    /// The target hit number of the crackable furni.
    /// </summary>
    int Target { get; }
}
