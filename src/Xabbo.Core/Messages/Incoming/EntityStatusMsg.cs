using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

public sealed record EntityStatusMsg(EntityStatusUpdate[] Updates) : IMessage<EntityStatusMsg>
{
    static Identifier IMessage<EntityStatusMsg>.Identifier => In.UserUpdate;

    static EntityStatusMsg IParser<EntityStatusMsg>.Parse(in PacketReader p) => new(p.ParseArray<EntityStatusUpdate>());
    void IComposer.Compose(in PacketWriter p) => p.ComposeArray(Updates);
}