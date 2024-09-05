using System;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

public record ChatMsg(ChatType Type, string Message, int BubbleStyle = 0, string Recipient = "") : IMessage<ChatMsg>
{
    static Identifier[] IMessage<ChatMsg>.Identifiers { get; } = [Out.Chat, Out.Shout, Out.Whisper];
    static Identifier IMessage<ChatMsg>.Identifier => default;

    Identifier IMessage<ChatMsg>.GetIdentifier(ClientType _) => Type switch
    {
        ChatType.Talk => Out.Chat,
        ChatType.Shout => Out.Shout,
        ChatType.Whisper => Out.Whisper,
        _ => throw new NotSupportedException($"Unknown chat type: {Type}.")
    };

    public ChatType Type { get; } = Type;

    static ChatMsg IParser<ChatMsg>.Parse(in PacketReader p)
    {
        if (p.Context is null)
            throw new NotSupportedException("Parser context is unavailable.");
        if (!p.Context.Messages.TryGetNames(p.Header, out var names))
            throw new NotSupportedException("Message header was not found.");
        ChatType chatType = names.Flash switch
        {
            nameof(Out.Chat) => ChatType.Talk,
            nameof(Out.Shout) => ChatType.Shout,
            nameof(Out.Whisper) => ChatType.Whisper,
            _ => throw new Exception("Unknown chat header."),
        };
        string message = p.ReadString();
        string recipient = "";
        int bubbleStyle = 0;
        if (chatType == ChatType.Whisper)
        {
            int index = message.IndexOf(' ');
            if (index >= 0)
            {
                recipient = message[..index];
                message = message[(index+1)..];
            }
        }
        if (p.Client is ClientType.Unity or ClientType.Flash)
            bubbleStyle = p.ReadInt();
        return new ChatMsg(chatType, message, bubbleStyle, recipient);
    }

    public void Compose(in PacketWriter p)
    {
        p.WriteString(Type == ChatType.Whisper ? $"{Recipient} {Message}" : Message);
        if (p.Client is not ClientType.Shockwave)
        {
            p.WriteInt(BubbleStyle);
            if (Type == ChatType.Talk)
                p.WriteInt(-1);
        }
    }
}
