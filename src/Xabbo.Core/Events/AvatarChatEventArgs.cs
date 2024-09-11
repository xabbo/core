namespace Xabbo.Core.Events;

public sealed class AvatarChatEventArgs(IAvatar avatar, ChatType chatType, string message, int bubbleStyle)
    : AvatarEventArgs(avatar)
{
    public ChatType ChatType { get; } = chatType;
    public string Message { get; } = message;
    public int BubbleStyle { get; } = bubbleStyle;
    public bool IsBlocked { get; private set; }
    public void Block() => IsBlocked = true;
}
