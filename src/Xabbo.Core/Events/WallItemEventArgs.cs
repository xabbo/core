using System;

namespace Xabbo.Core.Events;

public class WallItemEventArgs : EventArgs
{
    public IWallItem Item { get; }

    public WallItemEventArgs(IWallItem item)
    {
        Item = item;
    }
}
