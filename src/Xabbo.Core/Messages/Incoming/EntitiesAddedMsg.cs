using System.Collections.Generic;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

public sealed class EntitiesAddedMsg : List<Entity>, IMessage<EntitiesAddedMsg>
{
    public EntitiesAddedMsg() { }
    public EntitiesAddedMsg(int capacity) : base(capacity) { }
    public EntitiesAddedMsg(IEnumerable<Entity> collection) : base(collection) { }

    static Identifier IMessage<EntitiesAddedMsg>.Identifier => In.Users;
    static EntitiesAddedMsg IParser<EntitiesAddedMsg>.Parse(in PacketReader p) => new(p.ParseArray<Entity>());
    void IComposer.Compose(in PacketWriter p) => p.ComposeArray(this);
}