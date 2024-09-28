using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Received when an avatar starts or stops dancing.
/// <para/>
/// Supported clients: <see cref="ClientType.Modern"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="In.Dance"/></item>
/// </list>
/// </summary>
/// <param name="Index">The avatar's index.</param>
/// <param name="Dance">The avatar's updated dance.</param>
public sealed record AvatarDanceMsg(int Index, AvatarDance Dance) : IMessage<AvatarDanceMsg>
{
    static ClientType IMessage<AvatarDanceMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<AvatarDanceMsg>.Identifier => In.Dance;

    static AvatarDanceMsg IParser<AvatarDanceMsg>.Parse(in PacketReader p) => new(p.ReadInt(), (AvatarDance)p.ReadInt());
    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteInt(Index);
        p.WriteInt((int)Dance);
    }
}
