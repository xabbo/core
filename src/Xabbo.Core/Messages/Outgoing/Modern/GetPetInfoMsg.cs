using Xabbo.Messages;
using Xabbo.Messages.Flash;
using Xabbo.Core.Messages.Incoming.Modern;

namespace Xabbo.Core.Messages.Outgoing.Modern;

/// <summary>
/// Request for <see cref="PetInfoMsg"/>.
/// </summary>
public sealed record GetPetInfoMsg(Id Id) : IRequestMessage<GetPetInfoMsg, PetInfoMsg, PetInfo>
{
    static Identifier IMessage<GetPetInfoMsg>.Identifier => Out.GetPetInfo;
    PetInfo IResponseData<PetInfoMsg, PetInfo>.GetData(PetInfoMsg msg) => msg.Info;
    static GetPetInfoMsg IParser<GetPetInfoMsg>.Parse(in PacketReader p) => new(p.ReadId());
    void IComposer.Compose(in PacketWriter p) => p.WriteId(Id);
}
