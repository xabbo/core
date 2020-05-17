using System;
using System.Threading.Tasks;

using Xabbo.Core.Messages;

namespace Xabbo.Core.Tasks
{
    public class GetPetInfoTask : InterceptorTask<PetInfo>
    {
        private readonly int petId;

        public GetPetInfoTask(IInterceptor interceptor, int petId)
            : base(interceptor)
        {
            this.petId = petId;
        }

        protected override Task OnExecuteAsync() => SendAsync(Out.RequestPetInfo);

        [InterceptIn("PetInfo")]
        private void HandlePetInfo(InterceptEventArgs e)
        {
            try
            {
                var petInfo = PetInfo.Parse(e.Packet);
                if (petInfo.Id == petId)
                {
                    e.Block();
                    SetResult(petInfo);
                }
            }
            catch (Exception ex) { SetException(ex); }
        }
    }
}
