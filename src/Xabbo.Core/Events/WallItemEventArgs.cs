namespace Xabbo.Core.Events;

public class WallItemEventArgs(IWallItem item)
{
    public IWallItem Item { get; } = item;
}
