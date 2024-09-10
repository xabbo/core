using System;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

public sealed record GiveRightsMsg(Id? Id = null, string? Name = null) : IMessage<GiveRightsMsg>
{
    static Identifier IMessage<GiveRightsMsg>.Identifier => Out.AssignRights;

    static GiveRightsMsg IParser<GiveRightsMsg>.Parse(in PacketReader p) => p.Client switch
    {
        ClientType.Shockwave => new GiveRightsMsg { Name = p.ReadString() },
        not ClientType.Shockwave => new GiveRightsMsg { Id = p.ReadId() }
    };

    void IComposer.Compose(in PacketWriter p)
    {
        if (p.Client is ClientType.Shockwave)
        {
            if (Name is null)
                throw new Exception($"{nameof(GiveRightsMsg)} requires the user's name on {p.Client}.");
            p.WriteString(Name);
        }
        else
        {
            if (Id is not { } id)
                throw new Exception($"{nameof(GiveRightsMsg)} requires the user's ID on {p.Client}.");
            p.WriteId(id);
        }
    }
}