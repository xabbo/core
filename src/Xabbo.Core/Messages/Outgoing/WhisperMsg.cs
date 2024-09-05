using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

public record WhisperMsg(string Recipient, string Message, int BubbleStyle = 0)
    : ChatMsg(ChatType.Whisper, Message, BubbleStyle, Recipient), IMessage<WhisperMsg>
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
            message = message[(index+1)..];
        }
        int bubbleStyle = p.Client is not ClientType.Shockwave ? p.ReadInt() : 0;
        return new(recipient, message, bubbleStyle);
    }
}