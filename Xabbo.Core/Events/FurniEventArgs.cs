using System;

namespace Xabbo.Core.Events
{
    public class FurniEventArgs : EventArgs
    {
        public IFurni Item { get; }

        public FurniEventArgs(IFurni item)
        {
            Item = item;
        }
    }
}
