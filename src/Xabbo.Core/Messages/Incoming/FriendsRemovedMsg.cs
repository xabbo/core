using System.Collections.Generic;

using Xabbo.Messages;
using Xabbo.Messages.Shockwave;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Represents a list of user IDs to be removed from the friend's list.
/// <para/>
/// Received when friends are removed from the user's friend list.
/// <para/>
/// Supported clients: <see cref="ClientType.Origins"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Shockwave: <see cref="Xabbo.Messages.Shockwave.In.REMOVE_BUDDY"/></item>
/// </list>
/// </summary>
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
