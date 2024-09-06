using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

public sealed record ShoutMsg(string Message, int BubbleStyle = 0)
    : ChatMsg(ChatType.Shout, Message, BubbleStyle), IMessage<ShoutMsg>
{
    static Identifier IMessage<ShoutMsg>.Identifier => Out.Shout;

    public new ChatType Type => base.Type;

    static ShoutMsg IParser<ShoutMsg>.Parse(in PacketReader p) => new(
        p.ReadString(),
        p.Client is ClientType.Shockwave ? 0 : p.ReadInt()
    );
}
