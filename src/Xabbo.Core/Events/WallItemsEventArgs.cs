using System.Collections.Generic;

namespace Xabbo.Core.Events;

/// <summary>
/// Provides data for the <see cref="Game.RoomManager.WallItemsLoaded"/> event.
/// </summary>
/// <param name="items">The list of wall items.</param>
public sealed class WallItemsEventArgs(IEnumerable<IWallItem> items)
{
    /// <summary>
    /// Gets the list of wall items that were loaded.
    /// </summary>
    public IReadOnlyCollection<IWallItem> Items { get; } = new List<IWallItem>(items).AsReadOnly();
}
