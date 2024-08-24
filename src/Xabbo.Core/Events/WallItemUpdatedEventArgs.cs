namespace Xabbo.Core.Events;

public sealed class WallItemUpdatedEventArgs(IWallItem previousItem, IWallItem updatedItem)
    : WallItemEventArgs(updatedItem)
{
    public IWallItem PreviousItem { get; } = previousItem;
}
