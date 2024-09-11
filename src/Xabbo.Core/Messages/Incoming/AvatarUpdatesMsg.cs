using System.Collections.Generic;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

public sealed class AvatarUpdatesMsg : List<AvatarStatusUpdate>, IMessage<AvatarUpdatesMsg>
{
    public AvatarUpdatesMsg() { }
    public AvatarUpdatesMsg(int capacity) : base(capacity) { }
    public AvatarUpdatesMsg(IEnumerable<AvatarStatusUpdate> collection) : base(collection) { }

    static Identifier IMessage<AvatarUpdatesMsg>.Identifier => In.UserUpdate;
    static AvatarUpdatesMsg IParser<AvatarUpdatesMsg>.Parse(in PacketReader p) => new(p.ParseArray<AvatarStatusUpdate>());
    void IComposer.Compose(in PacketWriter p) => p.ComposeArray(this);
}