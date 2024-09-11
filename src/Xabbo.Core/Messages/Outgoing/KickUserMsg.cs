using System;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

public sealed record KickUserMsg(Id? Id = null, string? Name = null) : IMessage<KickUserMsg>
{
    static Identifier IMessage<KickUserMsg>.Identifier => Out.KickUser;

    static KickUserMsg IParser<KickUserMsg>.Parse(in PacketReader p) => p.Client switch
    {
        ClientType.Shockwave => new KickUserMsg { Name = p.ReadString() },
        not ClientType.Shockwave => new KickUserMsg { Id = p.ReadId() }
    };

    public KickUserMsg(IUser user) : this(user.Id, user.Name) { }

    void IComposer.Compose(in PacketWriter p)
    {
        if (p.Client is ClientType.Shockwave)
        {
            if (Name is null)
                throw new Exception($"{nameof(KickUserMsg)} requires the user's name on {p.Client}.");
            p.WriteString(Name);
        }
        else
        {
            if (Id is not { } id)
                throw new Exception($"{nameof(KickUserMsg)} requires the user's ID on {p.Client}.");
            p.WriteId(id);
        }
    }
}