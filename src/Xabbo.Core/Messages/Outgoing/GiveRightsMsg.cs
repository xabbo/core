using System;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

/// <summary>
/// Sent when giving rights to a user in the room.
/// <para/>
/// Supported clients: <see cref="ClientType.All"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="Out.AssignRights"/></item>
/// <item>Shockwave: <see cref="Xabbo.Messages.Shockwave.Out.ASSIGNRIGHTS"/></item>
/// </list>
/// </summary>
/// <param name="Id">The ID of the user to give rights to. Applies to <see cref="ClientType.Modern"/> clients.</param>
/// <param name="Name">The name of the user to give rights to. Applies to the <see cref="ClientType.Origins"/> client.</param>
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
