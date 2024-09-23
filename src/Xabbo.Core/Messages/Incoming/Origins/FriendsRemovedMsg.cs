using System.Collections.Generic;

using Xabbo.Messages;
using Xabbo.Messages.Shockwave;

namespace Xabbo.Core.Messages.Incoming.Origins;

public sealed class FriendsRemovedMsg : List<Id>, IMessage<FriendsRemovedMsg>
{
    static ClientType IMessage<FriendsRemovedMsg>.SupportedClients => ClientType.Origins;
    static Identifier IMessage<FriendsRemovedMsg>.Identifier => In.REMOVE_BUDDY;

    public FriendsRemovedMsg() { }
    public FriendsRemovedMsg(int capacity) : base(capacity) { }
    public FriendsRemovedMsg(IEnumerable<Id> collection) : base(collection) { }

    static FriendsRemovedMsg IParser<FriendsRemovedMsg>.Parse(in PacketReader p) => [.. p.ReadIdArray()];
    void IComposer.Compose(in PacketWriter p) => p.WriteIdArray(this);
}
