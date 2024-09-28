using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Received after requesting a pet's information.
/// <para/>
/// Response for <see cref="Outgoing.GetPetInfoMsg"/>.
/// <para/>
/// Supported clients: <see cref="ClientType.Modern"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="In.PetInfo"/></item>
/// </list>
/// </summary>
/// <param name="Info">The requested pet's information.</param>
public sealed record PetInfoMsg(PetInfo Info) : IMessage<PetInfoMsg>
{
    static ClientType IMessage<PetInfoMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<PetInfoMsg>.Identifier => In.PetInfo;
    static PetInfoMsg IParser<PetInfoMsg>.Parse(in PacketReader p) => new(p.Parse<PetInfo>());
    void IComposer.Compose(in PacketWriter p) => p.Compose(Info);
}
