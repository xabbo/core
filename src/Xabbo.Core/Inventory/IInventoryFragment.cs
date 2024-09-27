using System.Collections.Generic;

namespace Xabbo.Core;

/// <summary>
/// Represents a fragment of inventory items received when loading the inventory.
/// </summary>
public interface IInventoryFragment : IReadOnlyCollection<IInventoryItem>
{
    /// <summary>
    /// Gets the total number of fragments.
    /// </summary>
    int Total { get; }

    /// <summary>
    /// Gets the index of this fragment.
    /// </summary>
    int Index { get; }

    /// <summary>
    /// Gets the floor items in the inventory.
    /// </summary>
    IEnumerable<IInventoryItem> FloorItems { get; }

    /// <summary>
    /// Gets the wall items in the inventory.
    /// </summary>
    IEnumerable<IInventoryItem> WallItems { get; }
}
