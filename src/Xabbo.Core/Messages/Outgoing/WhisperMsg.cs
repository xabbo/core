using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

public sealed record WhisperMsg(string Recipient, string Message, int BubbleStyle = 0)
    : ChatMsg(ChatType.Whisper, Message, BubbleStyle, Recipient), IMessage<WhisperMsg>
{
    static Identifier IMessage<WhisperMsg>.Identifier => Out.Whisper;

    public new ChatType Type => base.Type;

    static WhisperMsg IParser<WhisperMsg>.Parse(in PacketReader p)
    {
        string message = p.ReadString();
        string recipient = "";
        int index = message.IndexOf(' ');
        if (index >= 0)
        {
            recipient = message[..index];
            message = message[(index+1)..];
        }
        int bubbleStyle = p.Client is ClientType.Shockwave ? 0 : p.ReadInt();
        return new(recipient, message, bubbleStyle);
    }
}