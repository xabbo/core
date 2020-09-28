using System;
using System.Threading.Tasks;

using Xabbo.Core.Messages;

namespace Xabbo.Core.Tasks
{
    [RequiredOut("RequestUserProfile")]
    public class GetProfileTask : InterceptorTask<UserProfile>
    {
        private readonly int userId;

        public GetProfileTask(IInterceptor interceptor, int userId)
            : base(interceptor)
        {
            this.userId = userId;
        }

        protected override Task OnExecuteAsync() => SendAsync(Out.RequestUserProfile, userId, false);

        [InterceptIn("UserProfile")]
        private void OnUserProfile(InterceptEventArgs e)
        {
            try
            {
                var userProfile = UserProfile.Parse(e.Packet);
                if (userProfile.Id == userId)
                {
                    if (SetResult(userProfile))
                        e.Block();
                }
            }
            catch (Exception ex) { SetException(ex); }
        }
    }
}
