using Xabbo.Messages;
using Xabbo.Messages.Flash;
using Xabbo.Core.Messages.Incoming.Modern;

namespace Xabbo.Core.Messages.Outgoing.Modern;

/// <summary>
/// Sent when requesting a pet's information.
/// <para/>
/// Request for <see cref="PetInfoMsg"/>. Returns a <see cref="PetInfo"/>.
/// <para/>
/// Supported clients: <see cref="ClientType.Modern"/>.
/// </summary>
/// <param name="Id">The ID of the pet to retrieve information for.</param>
public sealed record GetPetInfoMsg(Id Id) : IRequestMessage<GetPetInfoMsg, PetInfoMsg, PetInfo>
{
    static ClientType IMessage<GetPetInfoMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<GetPetInfoMsg>.Identifier => Out.GetPetInfo;
    PetInfo IResponseData<PetInfoMsg, PetInfo>.GetData(PetInfoMsg msg) => msg.Info;
    static GetPetInfoMsg IParser<GetPetInfoMsg>.Parse(in PacketReader p) => new(p.ReadId());
    void IComposer.Compose(in PacketWriter p) => p.WriteId(Id);
}
