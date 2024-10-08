using System.Collections.Generic;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

public sealed class RemoveFriendsMsg : List<Id>, IMessage<RemoveFriendsMsg>
{
    public RemoveFriendsMsg() { }
    public RemoveFriendsMsg(int capacity) : base(capacity) { }
    public RemoveFriendsMsg(IEnumerable<Id> ids) : base(ids) { }

    static Identifier IMessage<RemoveFriendsMsg>.Identifier => Out.RemoveFriend;
    static RemoveFriendsMsg IParser<RemoveFriendsMsg>.Parse(in PacketReader p) => [.. p.ReadIdArray()];
    void IComposer.Compose(in PacketWriter p) => p.WriteIdArray(this);
}