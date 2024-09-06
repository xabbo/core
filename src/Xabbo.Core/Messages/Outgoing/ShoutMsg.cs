using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

public sealed record ShoutMsg(string Message, int BubbleStyle = 0) : IMessage<ShoutMsg>
{
    static Identifier IMessage<ShoutMsg>.Identifier => Out.Shout;

    static ShoutMsg IParser<ShoutMsg>.Parse(in PacketReader p) => new(
        p.ReadString(),
        p.Client is ClientType.Shockwave ? 0 : p.ReadInt()
    );

    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteString(Message);
        if (p.Client is not ClientType.Shockwave)
            p.WriteInt(BubbleStyle);
    }
}
