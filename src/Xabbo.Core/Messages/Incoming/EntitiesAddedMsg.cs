using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

public sealed record EntitiesAddedMsg(Entity[] Entities) : IMessage<EntitiesAddedMsg>
{
    public static Identifier Identifier => In.Users;
    static EntitiesAddedMsg IParser<EntitiesAddedMsg>.Parse(in PacketReader p) => new(p.ParseArray<Entity>());
    void IComposer.Compose(in PacketWriter p) => p.ComposeArray(Entities);
}