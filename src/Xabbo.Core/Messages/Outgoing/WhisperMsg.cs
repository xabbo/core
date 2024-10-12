using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

/// <summary>
/// Sent when whispering in a room.
/// <para/>
/// Supported clients: <see cref="ClientType.All"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="Out.Whisper"/></item>
/// <item>Shockwave: <see cref="Xabbo.Messages.Shockwave.Out.WHISPER"/></item>
/// </list>
/// </summary>
/// <param name="Recipient">The recipient of the message.</param>
/// <param name="Message">The chat message content.</param>
/// <param name="BubbleStyle">The chat bubble style. Applies to <see cref="ClientType.Modern"/> clients.</param>
public sealed record WhisperMsg(string Recipient, string Message, int BubbleStyle = 0) : IMessage<WhisperMsg>
{
    static Identifier IMessage<WhisperMsg>.Identifier => Out.Whisper;

    static WhisperMsg IParser<WhisperMsg>.Parse(in PacketReader p)
    {
        string message = p.ReadString();
        string recipient = "";
        int index = message.IndexOf(' ');
        if (index >= 0)
        {
            recipient = message[..index];
            message = message[(index + 1)..];
        }
        int bubbleStyle = p.Client is ClientType.Shockwave ? 0 : p.ReadInt();
        return new(recipient, message, bubbleStyle);
    }

    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteString($"{Recipient} {Message}");
        if (p.Client is not ClientType.Shockwave)
            p.WriteInt(BubbleStyle);
    }
}
