using System.Collections.Generic;

using Xabbo.Messages;
using Xabbo.Messages.Shockwave;

namespace Xabbo.Core.Messages.Incoming.Origins;

public sealed class FriendsRemovedMsg : List<Id>, IMessage<FriendsRemovedMsg>
{
    static bool IMessage<FriendsRemovedMsg>.UseTargetedIdentifiers => true;
    static Identifier IMessage<FriendsRemovedMsg>.Identifier => In.REMOVE_BUDDY;

    public FriendsRemovedMsg() { }
    public FriendsRemovedMsg(int capacity) : base(capacity) { }
    public FriendsRemovedMsg(IEnumerable<Id> collection) : base(collection) { }

    static FriendsRemovedMsg IParser<FriendsRemovedMsg>.Parse(in PacketReader p)
    {
        UnsupportedClientException.ThrowIfNoneOr(p.Client, ~ClientType.Shockwave);
        return [.. p.ReadIdArray()];
    }

    void IComposer.Compose(in PacketWriter p)
    {
        UnsupportedClientException.ThrowIfNoneOr(p.Client, ~ClientType.Shockwave);
        p.WriteIdArray(this);
    }
}