using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

public sealed record TalkMsg(string Message, int BubbleStyle = 0)
    : ChatMsg(ChatType.Talk, Message, BubbleStyle), IMessage<TalkMsg>
{
    static Identifier IMessage<TalkMsg>.Identifier => Out.Chat;

    public new ChatType Type => base.Type;

    static TalkMsg IParser<TalkMsg>.Parse(in PacketReader p) => new(
        p.ReadString(),
        p.Client is ClientType.Shockwave ? 0 : p.ReadInt()
    );
}
