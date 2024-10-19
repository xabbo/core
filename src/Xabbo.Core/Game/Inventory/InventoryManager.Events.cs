using System;

using Xabbo.Core.Events;

namespace Xabbo.Core.Game;

partial class InventoryManager
{
    /// <summary>
    /// Occurs when the user's inventory is invalidated and needs to be reloaded.
    /// </summary>
    public event Action? Invalidated;

    /// <summary>
    /// Occurs when the user's inventory is loaded.
    /// </summary>
    public event Action? Loaded;

    /// <summary>
    /// Occurs when the inventory is cleared due to being reloaded or disconnected.
    /// </summary>
    public event Action? Cleared;

    /// <summary>
    /// Occurs when an item is added to the user's inventory.
    /// </summary>
    public event Action<InventoryItemEventArgs>? ItemAdded;

    /// <summary>
    /// Occurs when an item in the user's inventory is updated.
    /// </summary>
    public event Action<InventoryItemEventArgs>? ItemUpdated;

    /// <summary>
    /// Occurs when an item removed from the user's inventory.
    /// </summary>
    public event Action<InventoryItemEventArgs>? ItemRemoved;
}