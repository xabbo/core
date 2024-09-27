namespace Xabbo.Core;

/// <summary>
/// Defines chat related settings for a room.
/// </summary>
public interface IChatSettings
{
    /// <summary>
    /// Specifies the chat flow behavior.
    /// </summary>
    ChatFlow Flow { get; }

    /// <summary>
    /// Specifies the chat bubble width;
    /// </summary>
    ChatBubbleWidth BubbleWidth { get; }

    /// <summary>
    /// Specifies the chat scroll speed.
    /// </summary>
    ChatScrollSpeed ScrollSpeed { get; }

    /// <summary>
    /// Specifies the distance at which users can hear each other talk.
    /// </summary>
    int TalkHearingDistance { get; }

    /// <summary>
    /// Specifies the chat flood protection level.
    /// </summary>
    ChatFloodProtection FloodProtection { get; }
}
