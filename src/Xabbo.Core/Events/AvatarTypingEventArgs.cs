namespace Xabbo.Core.Events;

public sealed class AvatarTypingEventArgs(IAvatar avatar, bool wasTyping)
    : AvatarEventArgs(avatar)
{
    public bool WasTyping { get; } = wasTyping;
}
