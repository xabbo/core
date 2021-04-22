using System;
using System.Threading.Tasks;

using Xabbo.Messages;
using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;

namespace Xabbo.Core.Tasks
{
    public class GetProfileTask : InterceptorTask<IUserProfile>
    {
        private readonly long _userId;

        public GetProfileTask(IInterceptor interceptor, long userId)
            : base(interceptor)
        {
            _userId = userId;
        }

        protected override Task OnExecuteAsync() => SendAsync(Out.GetExtendedProfile, _userId, false);

        [InterceptIn(nameof(Incoming.ExtendedProfile))]
        protected void OnUserProfile(InterceptArgs e)
        {
            try
            {
                var userProfile = UserProfile.Parse(e.Packet);
                if (userProfile.Id == _userId)
                {
                    if (SetResult(userProfile))
                        e.Block();
                }
            }
            catch (Exception ex) { SetException(ex); }
        }
    }
}
