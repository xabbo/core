using System;
using System.ComponentModel;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

[EditorBrowsable(EditorBrowsableState.Advanced)]
public abstract record AvatarChatMsgBase(ChatType Type)
{
    public int AvatarIndex { get; init; }
    public string Message { get; init; } = "";
    public int BubbleStyle { get; init; }
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

        AvatarIndex = p.ReadInt();
        Message = p.ReadString();

        if (p.Client is not ClientType.Shockwave)
        {
            Gesture = p.ReadInt();
            BubbleStyle = p.ReadInt();

            int n = p.ReadLength();
            Links = new (string, string, bool)[n];
            for (int i = 0; i < n; i++)
                Links[i] = (p.ReadString(), p.ReadString(), p.ReadBool());

            TrackingId = p.ReadInt();
        }
    }

    protected void Compose(in PacketWriter p)
    {
        p.WriteInt(AvatarIndex);
        p.WriteString(Message);
        if (p.Client is not ClientType.Shockwave)
        {
            p.WriteInt(Gesture);
            p.WriteInt(BubbleStyle);
            p.WriteLength((Length)Links.Length);
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
/// <para/>
/// Supported clients: <see cref="ClientType.All"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="In.Chat"/>, <see cref="In.Whisper"/>, <see cref="In.Shout"/></item>
/// <item>
/// Shockwave:
/// <see cref="Xabbo.Messages.Shockwave.In.CHAT"/>,
/// <see cref="Xabbo.Messages.Shockwave.In.CHAT_2"/>,
/// <see cref="Xabbo.Messages.Shockwave.In.CHAT_3"/>
/// </item>
/// </list>
/// </summary>
public sealed record AvatarChatMsg : AvatarChatMsgBase, IMessage<AvatarChatMsg>
{
    static Identifier[] IMessage<AvatarChatMsg>.Identifiers { get; } = [In.Chat, In.Shout, In.Whisper];
    static Identifier IMessage<AvatarChatMsg>.Identifier => default;
    Identifier IMessage.GetIdentifier(ClientType client) => Type switch
    {
        ChatType.Talk => In.Chat,
        ChatType.Shout => In.Shout,
        ChatType.Whisper => In.Whisper,
        _ => throw new Exception($"Unknown chat type: {Type}.")
    };

    public AvatarChatMsg(ChatType Type, string Message, int AvatarIndex = -1, int BubbleStyle = 0)
        : base(Type)
    {
        base.AvatarIndex = AvatarIndex;
        base.Message = Message;
        base.BubbleStyle = BubbleStyle;
    }

    private AvatarChatMsg(in PacketReader p)
        : base((ChatType)(-1), in p)
    { }

    static AvatarChatMsg IParser<AvatarChatMsg>.Parse(in PacketReader p) => new(in p);
    void IComposer.Compose(in PacketWriter p) => Compose(in p);
}

/// <summary>
/// Received when an avatar in the room sends a talk message.
/// <para/>
/// Supported clients: <see cref="ClientType.All"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="In.Chat"/></item>
/// <item>Shockwave: <see cref="Xabbo.Messages.Shockwave.In.CHAT"/></item>
/// </list>
/// </summary>
public sealed record AvatarTalkMsg : AvatarChatMsgBase, IMessage<AvatarTalkMsg>
{
    static Identifier IMessage<AvatarTalkMsg>.Identifier => In.Chat;
    public new ChatType Type { get; } = ChatType.Talk;

    public AvatarTalkMsg()
        : base(ChatType.Talk)
    { }

    public AvatarTalkMsg(string Message, int AvatarIndex = -1, int BubbleStyle = 0)
        : this()
    {
        base.Message = Message;
        base.AvatarIndex = AvatarIndex;
        base.BubbleStyle = BubbleStyle;
    }

    private AvatarTalkMsg(in PacketReader p)
        : base(ChatType.Talk, in p)
    { }

    static AvatarTalkMsg IParser<AvatarTalkMsg>.Parse(in PacketReader p) => new(in p);
    void IComposer.Compose(in PacketWriter p) => Compose(in p);
}

/// <summary>
/// Received when an avatar in the room sends a shout message.
/// <para/>
/// Supported clients: <see cref="ClientType.All"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="In.Shout"/></item>
/// <item>Shockwave: <see cref="Xabbo.Messages.Shockwave.In.CHAT_3"/></item>
/// </list>
/// </summary>
public sealed record AvatarShoutMsg : AvatarChatMsgBase, IMessage<AvatarShoutMsg>
{
    static Identifier IMessage<AvatarShoutMsg>.Identifier => In.Shout;
    public new ChatType Type { get; } = ChatType.Shout;

    public AvatarShoutMsg()
        : base(ChatType.Shout)
    { }

    public AvatarShoutMsg(string Message, int AvatarIndex = -1, int BubbleStyle = 0)
        : this()
    {
        base.Message = Message;
        base.AvatarIndex = AvatarIndex;
        base.BubbleStyle = BubbleStyle;
    }

    private AvatarShoutMsg(in PacketReader p)
        : base(ChatType.Shout, in p)
    { }

    static AvatarShoutMsg IParser<AvatarShoutMsg>.Parse(in PacketReader p) => new(in p);
    void IComposer.Compose(in PacketWriter p) => Compose(in p);
}

/// <summary>
/// Received when an avatar in the room sends a whisper message.
/// <para/>
/// Supported clients: <see cref="ClientType.All"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="In.Whisper"/></item>
/// <item>Shockwave: <see cref="Xabbo.Messages.Shockwave.In.CHAT_2"/></item>
/// </list>
/// </summary>
public sealed record AvatarWhisperMsg : AvatarChatMsgBase, IMessage<AvatarWhisperMsg>
{
    static Identifier IMessage<AvatarWhisperMsg>.Identifier => In.Whisper;
    public new ChatType Type { get; } = ChatType.Whisper;

    public AvatarWhisperMsg()
        : base(ChatType.Whisper)
    { }

    public AvatarWhisperMsg(string Message, int AvatarIndex = -1, int BubbleStyle = 0)
        : this()
    {
        base.Message = Message;
        base.AvatarIndex = AvatarIndex;
        base.BubbleStyle = BubbleStyle;
    }

    private AvatarWhisperMsg(in PacketReader p)
        : base(ChatType.Whisper, in p)
    { }

    static AvatarWhisperMsg IParser<AvatarWhisperMsg>.Parse(in PacketReader p) => new(in p);
    void IComposer.Compose(in PacketWriter p) => Compose(in p);
}
