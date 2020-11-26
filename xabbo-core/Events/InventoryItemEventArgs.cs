using System;

namespace Xabbo.Core.Events
{
    public class InventoryItemEventArgs : EventArgs
    {
        public IInventoryItem Item { get; }
        public InventoryItemEventArgs(IInventoryItem item)
        {
            Item = item;
        }
    }
}
