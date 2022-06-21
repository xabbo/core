using System;
using System.Threading.Tasks;

using Xabbo.Messages;
using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;
using Xabbo.Interceptor.Attributes;

namespace Xabbo.Core.Tasks
{
    public class GetPetInfoTask : InterceptorTask<PetInfo>
    {
        private readonly long _petId;

        public GetPetInfoTask(IInterceptor interceptor, long petId)
            : base(interceptor)
        {
            _petId = petId;
        }

        protected override ValueTask OnExecuteAsync() => Interceptor.SendAsync(Out.GetNewPetInfo, (LegacyLong)_petId);

        [InterceptIn(nameof(Incoming.PetInfo))]
        protected void OnPetInfo(InterceptArgs e)
        {
            try
            {
                var petInfo = PetInfo.Parse(e.Packet);
                if (petInfo.Id == _petId)
                {
                    if (SetResult(petInfo))
                        e.Block();
                }
            }
            catch (Exception ex) { SetException(ex); }
        }
    }
}
