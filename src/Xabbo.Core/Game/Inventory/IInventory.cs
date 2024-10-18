using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Xabbo.Core.Game;

/// <summary>
/// Represents an inventory of items.
/// </summary>
public interface IInventory : IEnumerable<IInventoryItem>
{
    /// <summary>
    /// Gets the number of items in the inventory.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets whether the inventory has been invalidated by the server.
    /// </summary>
    bool IsInvalidated { get; }

    /// <summary>
    /// Attempts to get the item with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the item to retrieve.</param>
    /// <returns>The item with the specified ID, or <c>null</c> if it does not exist.</returns>
    IInventoryItem? GetItem(Id id);

    /// <summary>
    /// Attempts to get the item with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the item to retrieve.</param>
    /// <param name="item">
    /// When this method returns, contains the item with the specified ID,
    /// or <c>null</c> if it does not exist.
    /// </param>
    /// <returns>true if the item was retrieved successfully, otherwise false.</returns>
    bool TryGetItem(Id id, [NotNullWhen(true)] out IInventoryItem? item);
}
