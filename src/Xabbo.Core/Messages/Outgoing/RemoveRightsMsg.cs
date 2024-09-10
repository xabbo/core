using System;
using System.Collections.Generic;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

public sealed record RemoveRightsMsg(List<Id>? UserIds = null, string? Name = null) : IMessage<RemoveRightsMsg>
{
    static Identifier IMessage<RemoveRightsMsg>.Identifier => Out.RemoveRights;

    static RemoveRightsMsg IParser<RemoveRightsMsg>.Parse(in PacketReader p) => p.Client switch
    {
        ClientType.Shockwave => new RemoveRightsMsg { Name = p.ReadContent() },
        not ClientType.Shockwave => new RemoveRightsMsg { UserIds = new(p.ReadIdArray()) },
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
            if (UserIds is null)
                throw new Exception($"{nameof(UserIds)} is required when composing {nameof(RemoveRightsMsg)} on {p.Client}.");
            p.WriteIdArray(UserIds);
        }
    }
}