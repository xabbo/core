using System.Collections.Generic;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Represents a list of avatar status updates.
/// <para/>
/// Received when avatars' statuses are updated in the room.
/// <para/>
/// Supported clients: <see cref="ClientType.All"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="In.UserUpdate"/></item>
/// <item>Shockwave: <see cref="Xabbo.Messages.Shockwave.In.STATUS"/></item>
/// </list>
/// </summary>
public sealed class AvatarStatusMsg : List<AvatarStatus>, IMessage<AvatarStatusMsg>
{
    public AvatarStatusMsg() { }
    public AvatarStatusMsg(int capacity) : base(capacity) { }
    public AvatarStatusMsg(IEnumerable<AvatarStatus> collection) : base(collection) { }

    static Identifier IMessage<AvatarStatusMsg>.Identifier => In.UserUpdate;
    static AvatarStatusMsg IParser<AvatarStatusMsg>.Parse(in PacketReader p) => new(p.ParseArray<AvatarStatus>());
    void IComposer.Compose(in PacketWriter p) => p.ComposeArray(this);
}
