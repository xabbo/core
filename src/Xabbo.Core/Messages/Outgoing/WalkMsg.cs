using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

public sealed record WalkMsg(int X, int Y) : IMessage<WalkMsg>
{
    static Identifier IMessage<WalkMsg>.Identifier => Out.MoveAvatar;

    static WalkMsg IParser<WalkMsg>.Parse(in PacketReader p) => p.Client switch
    {
        not ClientType.Shockwave => new(p.ReadInt(), p.ReadInt()),
        ClientType.Shockwave => new(p.ReadB64(), p.ReadB64()),
    };

    void IComposer.Compose(in PacketWriter p)
    {
        switch (p.Client)
        {
        case not ClientType.Shockwave:
            p.WriteInt(X);
            p.WriteInt(Y);
            break;
        case ClientType.Shockwave:
            p.WriteB64((B64)X);
            p.WriteB64((B64)Y);
            break;
        }
    }
}