using System;
using System.Threading.Tasks;

using Xabbo.Core.Messages;

namespace Xabbo.Core.Tasks
{
    // @Update [RequiredOut(nameof(Outgoing.RequestUserProfile))]
    public class GetProfileTask : InterceptorTask<IUserProfile>
    {
        private readonly int userId;

        public GetProfileTask(IInterceptor interceptor, int userId)
            : base(interceptor)
        {
            this.userId = userId;
        }

        protected override Task OnExecuteAsync() => throw new NotImplementedException(); // @Update SendAsync(Out.RequestUserProfile, userId, false);

        // @Update [InterceptIn(nameof(Incoming.UserProfile))]
        protected void OnUserProfile(InterceptArgs e)
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
