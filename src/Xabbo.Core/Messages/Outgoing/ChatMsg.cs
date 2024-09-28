using System;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

/// <summary>
/// Sent when sending a chat message in a room.
/// <para/>
/// Supported clients: <see cref="ClientType.All"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="Out.Chat"/>, <see cref="Out.Whisper"/>, <see cref="Out.Shout"/></item>
/// <item>
/// Shockwave:
/// <see cref="Xabbo.Messages.Shockwave.Out.CHAT"/>,
/// <see cref="Xabbo.Messages.Shockwave.Out.WHISPER"/>,
/// <see cref="Xabbo.Messages.Shockwave.Out.SHOUT"/>.
/// </item>
/// </list>
/// </summary>
/// <param name="Type">The type of the chat message.</param>
/// <param name="Message">The chat message content.</param>
/// <param name="BubbleStyle">The chat bubble style. Applies to <see cref="ClientType.Modern"/> clients.</param>
/// <param name="Recipient">The recipient of the message, if this is a <see cref="ChatType.Whisper"/> message.</param>
public sealed record ChatMsg(ChatType Type, string Message, int BubbleStyle = 0, string Recipient = "") : IMessage<ChatMsg>
{
    static Identifier[] IMessage<ChatMsg>.Identifiers { get; } = [Out.Chat, Out.Shout, Out.Whisper];
    static Identifier IMessage<ChatMsg>.Identifier => default;

    Identifier IMessage.GetIdentifier(ClientType _) => Type switch
    {
        ChatType.Talk => Out.Chat,
        ChatType.Shout => Out.Shout,
        ChatType.Whisper => Out.Whisper,
        _ => throw new NotSupportedException($"Unknown chat type: {Type}.")
    };

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
                message = message[(index + 1)..];
            }
        }
        if (p.Client is ClientType.Unity or ClientType.Flash)
            bubbleStyle = p.ReadInt();

        return new ChatMsg(chatType, message, bubbleStyle, recipient);
    }

    void IComposer.Compose(in PacketWriter p)
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
