using System.Linq;
using System.Collections.Generic;

namespace Xabbo.Core.Events;

/// <summary>
/// Provides data for the <see cref="Game.RoomManager.FloorItemsLoaded"/> event.
/// </summary>
/// <param name="items">The floor items.</param>
public sealed class FloorItemsEventArgs(IEnumerable<IFloorItem> items)
{
    /// <summary>
    /// Gets the floor items.
    /// </summary>
    public IFloorItem[] Items { get; } = items.ToArray();
}
