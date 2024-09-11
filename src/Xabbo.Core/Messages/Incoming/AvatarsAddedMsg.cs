using System.Collections.Generic;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

public sealed class AvatarsAddedMsg : List<Avatar>, IMessage<AvatarsAddedMsg>
{
    public AvatarsAddedMsg() { }
    public AvatarsAddedMsg(int capacity) : base(capacity) { }
    public AvatarsAddedMsg(IEnumerable<Avatar> collection) : base(collection) { }

    static Identifier IMessage<AvatarsAddedMsg>.Identifier => In.Users;
    static AvatarsAddedMsg IParser<AvatarsAddedMsg>.Parse(in PacketReader p) => new(p.ParseArray<Avatar>());
    void IComposer.Compose(in PacketWriter p) => p.ComposeArray(this);
}