using System;
using System.Threading.Tasks;

using Xabbo.Messages;
using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;

namespace Xabbo.Core.Tasks
{
    // @Update [RequiredOut(nameof(Outgoing.RequestPetInfo))]
    public class GetPetInfoTask : InterceptorTask<PetInfo>
    {
        private readonly int petId;

        public GetPetInfoTask(IInterceptor interceptor, int petId)
            : base(interceptor)
        {
            this.petId = petId;
        }

        protected override Task OnExecuteAsync() => throw new NotImplementedException(); // @Update SendAsync(Out.RequestPetInfo);

        // @Update [InterceptIn(nameof(Incoming.PetInfo))]
        protected void OnPetInfo(InterceptArgs e)
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
