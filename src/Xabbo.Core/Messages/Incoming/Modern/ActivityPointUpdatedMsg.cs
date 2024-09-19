using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming.Modern;

public sealed record ActivityPointUpdatedMsg(ActivityPointType Type, int Amount, int Change)
    : IMessage<ActivityPointUpdatedMsg>
{
    static bool IMessage<ActivityPointUpdatedMsg>.UseTargetedIdentifiers => true;
    static Identifier IMessage<ActivityPointUpdatedMsg>.Identifier => In.HabboActivityPointNotification;

    static ActivityPointUpdatedMsg IParser<ActivityPointUpdatedMsg>.Parse(in PacketReader p)
    {
        UnsupportedClientException.ThrowIfOrigins(p.Client);

        int amount = p.ReadInt();
        int change = p.ReadInt();
        var type = (ActivityPointType)p.ReadInt();
        return new(type, amount, change);
    }

    void IComposer.Compose(in PacketWriter p)
    {
        UnsupportedClientException.ThrowIfOrigins(p.Client);

        p.WriteInt(Amount);
        p.WriteInt(Change);
        p.WriteInt((int)Type);
    }
}