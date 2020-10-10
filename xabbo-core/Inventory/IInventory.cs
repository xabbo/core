using System;
using System.Collections.Generic;

namespace Xabbo.Core
{
    public interface IInventory : IReadOnlyCollection<IInventoryItem>
    {
        /// <summary>
        /// Gets the floor items in the inventory.
        /// </summary>
        IEnumerable<IInventoryItem> FloorItems { get; }

        /// <summary>
        /// Gets the wall items in the inventory.
        /// </summary>
        IEnumerable<IInventoryItem> WallItems { get; }
    }
}
