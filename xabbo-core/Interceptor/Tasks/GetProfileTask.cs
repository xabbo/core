using System;
using System.Threading.Tasks;

using Xabbo.Core.Messages;

namespace Xabbo.Core.Tasks
{
    [RequiredOut(nameof(Outgoing.RequestUserProfile))]
    public class GetProfileTask : InterceptorTask<IUserProfile>
    {
        private readonly int userId;

        public GetProfileTask(IInterceptor interceptor, int userId)
            : base(interceptor)
        {
            this.userId = userId;
        }

        protected override Task OnExecuteAsync() => SendAsync(Out.RequestUserProfile, userId, false);

        [InterceptIn(nameof(Incoming.UserProfile))]
        protected void OnUserProfile(InterceptEventArgs e)
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
