using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Received when an avatar's effect is updated.
/// <para/>
/// Supported clients: <see cref="ClientType.Modern"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="In.AvatarEffect"/></item>
/// </list>
/// </summary>
/// <param name="Index">The index of the avatar.</param>
/// <param name="Effect">The avatar's updated effect ID.</param>
/// <param name="Delay">The effect delay.</param>
public sealed record AvatarEffectMsg(int Index, int Effect, int Delay = 0) : IMessage<AvatarEffectMsg>
{
    static ClientType IMessage<AvatarEffectMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<AvatarEffectMsg>.Identifier => In.AvatarEffect;

    static AvatarEffectMsg IParser<AvatarEffectMsg>.Parse(in PacketReader p) => new(p.ReadInt(), p.ReadInt(), p.ReadInt());
    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteInt(Index);
        p.WriteInt(Effect);
        p.WriteInt(Delay);
    }
}
