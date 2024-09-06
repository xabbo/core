using System;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

public abstract record EntityChatMsgBase(ChatType Type)
{
    public int Index { get; init; }
    public string Message { get; init; } = "";
    public int Style { get; init; }
    public int Gesture { get; init; }
    public (string, string Url, bool)[] Links { get; init; } = [];
    public int TrackingId { get; init; }

    protected EntityChatMsgBase(ChatType type, in PacketReader p)
        : this(type)
    {
        if (Type == (ChatType)(-1))
        {
            // Get chat type from parser context.
            if (p.Context is null)
                throw new NullReferenceException("Context is null when parsing entity chat message.");
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
/// Received when an entity in the room sends a chat message.
/// </summary>
public sealed record EntityChatMsg : EntityChatMsgBase, IMessage<EntityChatMsg>
{
    static Identifier[] IMessage<EntityChatMsg>.Identifiers { get; } = [ In.Chat, In.Shout, In.Whisper ];
    static Identifier IMessage<EntityChatMsg>.Identifier => default;
    Identifier IMessage<EntityChatMsg>.GetIdentifier(ClientType client) => Type switch
    {
        ChatType.Talk => In.Chat,
        ChatType.Shout => In.Shout,
        ChatType.Whisper => In.Whisper,
        _ => throw new Exception($"Unknown chat type: {Type}.")
    };

    public EntityChatMsg(ChatType type, int index, string message, int style = 0)
        : base(type)
    {
        Index = index;
        Message = message;
        Style = style;
    }

    private EntityChatMsg(in PacketReader p)
        : base((ChatType)(-1), in p)
    { }

    static EntityChatMsg IParser<EntityChatMsg>.Parse(in PacketReader p) => new(in p);
    void IComposer.Compose(in PacketWriter p) => Compose(in p);
}

public sealed record EntityTalkMsg : EntityChatMsgBase, IMessage<EntityTalkMsg>
{
    static Identifier IMessage<EntityTalkMsg>.Identifier => In.Chat;
    public new ChatType Type { get; } = ChatType.Talk;

    public EntityTalkMsg()
        : base(ChatType.Talk)
    { }

    public EntityTalkMsg(int index, string message, int style = 0)
        : this()
    {
        Index = index;
        Message = message;
        Style = style;
    }

    private EntityTalkMsg(in PacketReader p)
        : base(ChatType.Talk, in p)
    { }

    static EntityTalkMsg IParser<EntityTalkMsg>.Parse(in PacketReader p) => new(in p);
    void IComposer.Compose(in PacketWriter p) => Compose(in p);
}

public sealed record EntityShoutMsg : EntityChatMsgBase, IMessage<EntityShoutMsg>
{
    static Identifier IMessage<EntityShoutMsg>.Identifier => In.Shout;
    public new ChatType Type { get; } = ChatType.Shout;

    public EntityShoutMsg()
        : base(ChatType.Shout)
    { }

    public EntityShoutMsg(int index, string message, int style = 0)
        : this()
    {
        Index = index;
        Message = message;
        Style = style;
    }

    private EntityShoutMsg(in PacketReader p)
        : base(ChatType.Shout, in p)
    { }

    static EntityShoutMsg IParser<EntityShoutMsg>.Parse(in PacketReader p) => new(in p);
    void IComposer.Compose(in PacketWriter p) => Compose(in p);
}

public sealed record EntityWhisperMsg : EntityChatMsgBase, IMessage<EntityWhisperMsg>
{
    static Identifier IMessage<EntityWhisperMsg>.Identifier => In.Whisper;
    public new ChatType Type { get; } = ChatType.Whisper;

    public EntityWhisperMsg()
        : base(ChatType.Whisper)
    { }

    public EntityWhisperMsg(int index, string message, int style = 0)
        : this()
    {
        Index = index;
        Message = message;
        Style = style;
    }

    private EntityWhisperMsg(in PacketReader p)
        : base(ChatType.Whisper, in p)
    { }

    static EntityWhisperMsg IParser<EntityWhisperMsg>.Parse(in PacketReader p) => new(in p);
    void IComposer.Compose(in PacketWriter p) => Compose(in p);
}
