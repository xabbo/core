namespace Xabbo.Core.Events;

public sealed class AvatarSlideEventArgs(IAvatar avatar, Tile previousTile)
    : AvatarEventArgs(avatar)
{
    public Tile PreviousTile { get; set; } = previousTile;
}
