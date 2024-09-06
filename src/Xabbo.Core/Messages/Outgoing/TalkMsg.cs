using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

public sealed record TalkMsg(string Message, int BubbleStyle = 0) : IMessage<TalkMsg>
{
    static Identifier IMessage<TalkMsg>.Identifier => Out.Chat;

    static TalkMsg IParser<TalkMsg>.Parse(in PacketReader p) => new(
        p.ReadString(),
        p.Client is ClientType.Shockwave ? 0 : p.ReadInt()
    );

    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteString(Message);
        if (p.Client is not ClientType.Shockwave)
        {
            p.WriteInt(BubbleStyle);
            p.WriteInt(-1);
        }
    }
}
