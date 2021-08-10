using System;
using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core
{
    public interface IInventoryFragment : IReadOnlyCollection<IInventoryItem>, IComposable
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
}
