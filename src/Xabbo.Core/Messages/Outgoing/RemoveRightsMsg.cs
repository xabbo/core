using System;
using System.Collections.Generic;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

/// <summary>
/// Sent when removing the rights of a user from the current room.
/// <para/>
/// Supported clients: <see cref="ClientType.All"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="Out.RemoveRights"/></item>
/// <item>Shockwave: <see cref="Xabbo.Messages.Shockwave.Out.REMOVERIGHTS"/></item>
/// </list>
/// </summary>
/// <param name="Ids">The list of user IDs to remove rights from. Applies to <see cref="ClientType.Modern"/> clients.</param>
/// <param name="Name">The name of the user to remove rights from. Applies to the <see cref="ClientType.Origins"/> client.</param>
public sealed record RemoveRightsMsg(List<Id>? Ids = null, string? Name = null) : IMessage<RemoveRightsMsg>
{
    static Identifier IMessage<RemoveRightsMsg>.Identifier => Out.RemoveRights;

    static RemoveRightsMsg IParser<RemoveRightsMsg>.Parse(in PacketReader p) => p.Client switch
    {
        ClientType.Shockwave => new RemoveRightsMsg { Name = p.ReadContent() },
        not ClientType.Shockwave => new RemoveRightsMsg { Ids = new(p.ReadIdArray()) },
    };

    void IComposer.Compose(in PacketWriter p)
    {
        if (p.Client is ClientType.Shockwave)
        {
            if (Name is null)
                throw new Exception($"{nameof(Name)} is required when composing {nameof(RemoveRightsMsg)} on {p.Client}.");
            p.WriteContent(Name);
        }
        else
        {
            if (Ids is null)
                throw new Exception($"{nameof(Ids)} is required when composing {nameof(RemoveRightsMsg)} on {p.Client}.");
            p.WriteIdArray(Ids);
        }
    }
}