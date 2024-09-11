using System;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

public abstract record AvatarChatMsgBase(ChatType Type)
{
    public int Index { get; init; }
    public string Message { get; init; } = "";
    public int Style { get; init; }
    public int Gesture { get; init; }
    public (string, string Url, bool)[] Links { get; init; } = [];
    public int TrackingId { get; init; }

    protected AvatarChatMsgBase(ChatType type, in PacketReader p)
        : this(type)
    {
        if (Type == (ChatType)(-1))
        {
            // Get chat type from parser context.
            if (p.Context is null)
                throw new NullReferenceException("Context is null when parsing avatar chat message.");
            if (p.Context.Messages.Is(p.Header, In.Chat))
                Type = ChatType.Talk;
            else if (p.Context.Messages.Is(p.Header, In.Shout))
                Type = ChatType.Shout;
            else if (p.Context.Messages.Is(p.Header, In.Whisper))
                Type = ChatType.Whisper;
            else
                throw new Exception("Failed to get chat type from parser context.");
        }

        Index = p.ReadInt();
        Message = p.ReadString();

        if (p.Client is not ClientType.Shockwave)
        {
            Gesture = p.ReadInt();
            Style = p.ReadInt();

            int n = p.ReadLength();
            Links = new (string, string, bool)[n];
            for (int i = 0; i < n; i++)
                Links[i] = (p.ReadString(), p.ReadString(), p.ReadBool());

            TrackingId = p.ReadInt();
        }
    }

    protected void Compose(in PacketWriter p)
    {
        p.WriteInt(Index);
        p.WriteString(Message);
        if (p.Client is not ClientType.Shockwave)
        {
            p.WriteInt(Gesture);
            p.WriteInt(Style);
            p.WriteLength(Links.Length);
            for (int i = 0; i < Links.Length; i++)
            {
                p.WriteString(Links[i].Item1);
                p.WriteString(Links[i].Url);
                p.WriteBool(Links[i].Item3);
            }
            p.WriteInt(TrackingId);
        }
    }
}

/// <summary>
/// Received when an avatar in the room sends a chat message.
/// </summary>
public sealed record AvatarChatMsg : AvatarChatMsgBase, IMessage<AvatarChatMsg>
{
    static Identifier[] IMessage<AvatarChatMsg>.Identifiers { get; } = [ In.Chat, In.Shout, In.Whisper ];
    static Identifier IMessage<AvatarChatMsg>.Identifier => default;
    Identifier IMessage<AvatarChatMsg>.GetIdentifier(ClientType client) => Type switch
    {
        ChatType.Talk => In.Chat,
        ChatType.Shout => In.Shout,
        ChatType.Whisper => In.Whisper,
        _ => throw new Exception($"Unknown chat type: {Type}.")
    };

    public AvatarChatMsg(ChatType type, int index, string message, int style = 0)
        : base(type)
    {
        Index = index;
        Message = message;
        Style = style;
    }

    private AvatarChatMsg(in PacketReader p)
        : base((ChatType)(-1), in p)
    { }

    static AvatarChatMsg IParser<AvatarChatMsg>.Parse(in PacketReader p) => new(in p);
    void IComposer.Compose(in PacketWriter p) => Compose(in p);
}

public sealed record AvatarTalkMsg : AvatarChatMsgBase, IMessage<AvatarTalkMsg>
{
    static Identifier IMessage<AvatarTalkMsg>.Identifier => In.Chat;
    public new ChatType Type { get; } = ChatType.Talk;

    public AvatarTalkMsg()
        : base(ChatType.Talk)
    { }

    public AvatarTalkMsg(int index, string message, int style = 0)
        : this()
    {
        Index = index;
        Message = message;
        Style = style;
    }

    private AvatarTalkMsg(in PacketReader p)
        : base(ChatType.Talk, in p)
    { }

    static AvatarTalkMsg IParser<AvatarTalkMsg>.Parse(in PacketReader p) => new(in p);
    void IComposer.Compose(in PacketWriter p) => Compose(in p);
}

public sealed record AvatarShoutMsg : AvatarChatMsgBase, IMessage<AvatarShoutMsg>
{
    static Identifier IMessage<AvatarShoutMsg>.Identifier => In.Shout;
    public new ChatType Type { get; } = ChatType.Shout;

    public AvatarShoutMsg()
        : base(ChatType.Shout)
    { }

    public AvatarShoutMsg(int index, string message, int style = 0)
        : this()
    {
        Index = index;
        Message = message;
        Style = style;
    }

    private AvatarShoutMsg(in PacketReader p)
        : base(ChatType.Shout, in p)
    { }

    static AvatarShoutMsg IParser<AvatarShoutMsg>.Parse(in PacketReader p) => new(in p);
    void IComposer.Compose(in PacketWriter p) => Compose(in p);
}

public sealed record AvatarWhisperMsg : AvatarChatMsgBase, IMessage<AvatarWhisperMsg>
{
    static Identifier IMessage<AvatarWhisperMsg>.Identifier => In.Whisper;
    public new ChatType Type { get; } = ChatType.Whisper;

    public AvatarWhisperMsg()
        : base(ChatType.Whisper)
    { }

    public AvatarWhisperMsg(int index, string message, int style = 0)
        : this()
    {
        Index = index;
        Message = message;
        Style = style;
    }

    private AvatarWhisperMsg(in PacketReader p)
        : base(ChatType.Whisper, in p)
    { }

    static AvatarWhisperMsg IParser<AvatarWhisperMsg>.Parse(in PacketReader p) => new(in p);
    void IComposer.Compose(in PacketWriter p) => Compose(in p);
}
