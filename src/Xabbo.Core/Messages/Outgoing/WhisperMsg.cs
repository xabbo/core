using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

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