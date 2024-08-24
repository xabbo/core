namespace Xabbo.Core.Events;

public sealed class EntityChatEventArgs(IEntity entity, ChatType chatType, string message, int bubbleStyle)
    : EntityEventArgs(entity)
{
    public ChatType ChatType { get; } = chatType;
    public string Message { get; } = message;
    public int BubbleStyle { get; } = bubbleStyle;
    public bool IsBlocked { get; private set; }
    public void Block() => IsBlocked = true;
}
