using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Received when an avatar performs an action.
/// <para/>
/// Supported clients: <see cref="ClientType.Modern"/>.
/// </summary>
/// <param name="Index">The avatar's index.</param>
/// <param name="Action">The action that the avatar peformed.</param>
public sealed record AvatarActionMsg(int Index, Actions Action) : IMessage<AvatarActionMsg>
{
    static ClientType IMessage<AvatarActionMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<AvatarActionMsg>.Identifier => In.Expression;

    static AvatarActionMsg IParser<AvatarActionMsg>.Parse(in PacketReader p) => new(p.ReadInt(), (Actions)p.ReadInt());
    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteInt(Index);
        p.WriteInt((int)Action);
    }
}