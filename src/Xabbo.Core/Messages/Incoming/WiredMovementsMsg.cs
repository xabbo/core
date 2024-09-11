using System.Collections.Generic;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

public sealed class WiredMovementsMsg : List<WiredMovement>, IMessage<WiredMovementsMsg>
{
    static bool IMessage<WiredMovementsMsg>.UseTargetedIdentifiers => true;
    static Identifier IMessage<WiredMovementsMsg>.Identifier => In.WiredMovements;

    public WiredMovementsMsg() { }
    public WiredMovementsMsg(int capacity) : base(capacity) { }
    public WiredMovementsMsg(IEnumerable<WiredMovement> collection) : base(collection) { }

    static WiredMovementsMsg IParser<WiredMovementsMsg>.Parse(in PacketReader p) => new(p.ParseArray<WiredMovement>());
    void IComposer.Compose(in PacketWriter p) => p.ComposeArray(this);
}