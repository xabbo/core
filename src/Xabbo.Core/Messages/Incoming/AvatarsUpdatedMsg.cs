using System.Collections.Generic;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

public sealed class AvatarsUpdatedMsg : List<AvatarStatusUpdate>, IMessage<AvatarsUpdatedMsg>
{
    public AvatarsUpdatedMsg() { }
    public AvatarsUpdatedMsg(int capacity) : base(capacity) { }
    public AvatarsUpdatedMsg(IEnumerable<AvatarStatusUpdate> collection) : base(collection) { }

    static Identifier IMessage<AvatarsUpdatedMsg>.Identifier => In.UserUpdate;
    static AvatarsUpdatedMsg IParser<AvatarsUpdatedMsg>.Parse(in PacketReader p) => new(p.ParseArray<AvatarStatusUpdate>());
    void IComposer.Compose(in PacketWriter p) => p.ComposeArray(this);
}