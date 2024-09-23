using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming.Modern;

/// <summary>
/// Response for <see cref="Outgoing.Modern.GetPetInfoMsg"/>.
/// </summary>
public sealed record PetInfoMsg(PetInfo Info) : IMessage<PetInfoMsg>
{
    static ClientType IMessage<PetInfoMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<PetInfoMsg>.Identifier => In.PetInfo;
    static PetInfoMsg IParser<PetInfoMsg>.Parse(in PacketReader p) => new(p.Parse<PetInfo>());
    void IComposer.Compose(in PacketWriter p) => p.Compose(Info);
}
