using System;
using System.Threading.Tasks;

using Xabbo.Core.Messages;

namespace Xabbo.Core.Tasks
{
    [RequiredOut(nameof(Outgoing.RequestPetInfo))]
    public class GetPetInfoTask : InterceptorTask<PetInfo>
    {
        private readonly int petId;

        public GetPetInfoTask(IInterceptor interceptor, int petId)
            : base(interceptor)
        {
            this.petId = petId;
        }

        protected override Task OnExecuteAsync() => SendAsync(Out.RequestPetInfo);

        [InterceptIn(nameof(Incoming.PetInfo))]
        protected void OnPetInfo(InterceptEventArgs e)
        {
            try
            {
                var petInfo = PetInfo.Parse(e.Packet);
                if (petInfo.Id == petId)
                {
                    if (SetResult(petInfo))
                        e.Block();
                }
            }
            catch (Exception ex) { SetException(ex); }
        }
    }
}
