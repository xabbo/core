using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

public record ShoutMsg(string Message, int BubbleStyle = 0)
    : ChatMsg(ChatType.Shout, Message, BubbleStyle), IMessage<ShoutMsg>
{
    static Identifier IMessage<ShoutMsg>.Identifier => Out.Shout;

    static ShoutMsg IParser<ShoutMsg>.Parse(in PacketReader p) => new(
        p.ReadString(),
        p.Client is not ClientType.Shockwave ? p.ReadInt() : 0
    );
}
