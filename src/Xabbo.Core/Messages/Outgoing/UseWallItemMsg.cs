using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

public sealed record UseWallItemMsg(Id Id, int State = 0) : IMessage<UseWallItemMsg>
{
    static Identifier IMessage<UseWallItemMsg>.Identifier => Out.UseWallItem;
    static bool IMessage<UseWallItemMsg>.UseTargetedIdentifiers => true;

    Identifier IMessage<UseWallItemMsg>.GetIdentifier(ClientType client) => client switch
    {
        ClientType.Flash => Out.UseWallItem,
        _ => throw new UnsupportedClientException(client),
    };

    static UseWallItemMsg IParser<UseWallItemMsg>.Parse(in PacketReader p)
    {
        Id id; int state;

        switch (p.Client)
        {
            case ClientType.Unity or ClientType.Flash:
                id = p.ReadId();
                state = p.ReadInt();
                break;
            default:
                throw new UnsupportedClientException(p.Client);
        }

        return new(id, state);
    }

    public void Compose(in PacketWriter p)
    {
        switch (p.Client)
        {
            case ClientType.Unity or ClientType.Flash:
                p.WriteId(Id);
                p.WriteInt(State);
                break;
            default:
                throw new UnsupportedClientException(p.Client);
        }
    }
}