using System;

using Xabbo.Messages.Flash;
using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;

namespace Xabbo.Core.Tasks;

[Intercepts]
public sealed partial class GetPetInfoTask(IInterceptor interceptor, Id petId)
    : InterceptorTask<PetInfo>(interceptor)
{
    private readonly Id _petId = petId;

    protected override void OnExecute() => Interceptor.Send(Out.GetPetInfo, _petId);

    [InterceptIn(nameof(In.PetInfo))]
    private void HandlePetInfo(Intercept e)
    {
        try
        {
            var petInfo = e.Packet.Read<PetInfo>();
            if (petInfo.Id == _petId)
            {
                if (SetResult(petInfo))
                    e.Block();
            }
        }
        catch (Exception ex) { SetException(ex); }
    }
}
