namespace Xabbo.Core.Events;

/// <summary>
/// Provides data for the <see cref="Game.RoomManager.AvatarChat"/> event.
/// </summary>
/// <param name="avatar">The avatar.</param>
/// <param name="chatType">The chat type.</param>
/// <param name="message">The message content.</param>
/// <param name="bubbleStyle">The chat bubble style.</param>
public sealed class AvatarChatEventArgs(IAvatar avatar, ChatType chatType, string message, int bubbleStyle)
    : AvatarEventArgs(avatar)
{
    /// <summary>
    /// Gets the chat type.
    /// </summary>
    public ChatType ChatType { get; } = chatType;

    /// <summary>
    /// Gets the message content.
    /// </summary>
    public string Message { get; } = message;

    /// <summary>
    /// Gets the chat bubble style.
    /// </summary>
    public int BubbleStyle { get; } = bubbleStyle;

    /// <summary>
    /// Gets whether the chat message has been blocked.
    /// </summary>
    public bool IsBlocked { get; private set; }

    /// <summary>
    /// Blocks the chat message.
    /// </summary>
    public void Block() => IsBlocked = true;
}
