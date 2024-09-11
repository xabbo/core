namespace Xabbo.Core.Events;

public sealed class AvatarNameChangedEventArgs(IAvatar avatar, string previousName)
    : AvatarEventArgs(avatar)
{
    public string PreviousName { get; } = previousName;
}
