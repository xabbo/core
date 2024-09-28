using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Received when an avatar starts or stops idling.
/// <para/>
/// Supported clients: <see cref="ClientType.Modern"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="In.Sleep"/></item>
/// </list>
/// </summary>
/// <param name="Index">The index of the avatar.</param>
/// <param name="Idle">Whether the avatar is idle.</param>
public sealed record AvatarIdleMsg(int Index, bool Idle) : IMessage<AvatarIdleMsg>
{
    static ClientType IMessage<AvatarIdleMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<AvatarIdleMsg>.Identifier => In.Sleep;

    static AvatarIdleMsg IParser<AvatarIdleMsg>.Parse(in PacketReader p) => new(p.ReadInt(), p.ReadBool());
    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteInt(Index);
        p.WriteBool(Idle);
    }
}
