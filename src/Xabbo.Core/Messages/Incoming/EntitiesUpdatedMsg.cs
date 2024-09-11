using System.Collections.Generic;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

public sealed class EntitiesUpdatedMsg : List<EntityStatusUpdate>, IMessage<EntitiesUpdatedMsg>
{
    public EntitiesUpdatedMsg() { }
    public EntitiesUpdatedMsg(int capacity) : base(capacity) { }
    public EntitiesUpdatedMsg(IEnumerable<EntityStatusUpdate> collection) : base(collection) { }

    static Identifier IMessage<EntitiesUpdatedMsg>.Identifier => In.UserUpdate;
    static EntitiesUpdatedMsg IParser<EntitiesUpdatedMsg>.Parse(in PacketReader p) => new(p.ParseArray<EntityStatusUpdate>());
    void IComposer.Compose(in PacketWriter p) => p.ComposeArray(this);
}