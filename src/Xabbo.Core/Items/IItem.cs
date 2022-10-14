using System;

namespace Xabbo.Core;

public interface IItem
{
    /// <summary>
    /// Gets the type of furni of the item.
    /// </summary>
    ItemType Type { get; }

    /// <summary>
    /// Gets the kind of furni of the item.
    /// </summary>
    int Kind { get; }

    /// <summary>
    /// Gets the ID of the item.
    /// </summary>
    long Id { get; }
}
