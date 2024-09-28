using System.Collections.Generic;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

/// <summary>
/// Sent when removing friends from the user's friends list.
/// <para/>
/// Supported clients: <see cref="ClientType.All"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="Out.RemoveFriend"/></item>
/// <item>Shockwave: <see cref="Xabbo.Messages.Shockwave.Out.FRIENDLIST_REMOVEFRIEND"/></item>
/// </list>
/// </summary>
public sealed class RemoveFriendsMsg : List<Id>, IMessage<RemoveFriendsMsg>
{
    public RemoveFriendsMsg() { }
    public RemoveFriendsMsg(int capacity) : base(capacity) { }
    public RemoveFriendsMsg(IEnumerable<Id> ids) : base(ids) { }

    static Identifier IMessage<RemoveFriendsMsg>.Identifier => Out.RemoveFriend;
    static RemoveFriendsMsg IParser<RemoveFriendsMsg>.Parse(in PacketReader p) => [.. p.ReadIdArray()];
    void IComposer.Compose(in PacketWriter p) => p.WriteIdArray(this);
}
