using System;
using System.Threading.Tasks;

using Xabbo.Core.Messages;

namespace Xabbo.Core.Tasks
{
    [RequiredOut("RequestAchievements")]
    public class GetAchievementsTask : InterceptorTask<IAchievements>
    {
        public GetAchievementsTask(IInterceptor interceptor)
            : base(interceptor)
        { }

        protected override Task OnExecuteAsync() => SendAsync(Out.RequestAchievements);

        [InterceptIn("AchievementList")]
        private void HandleAchievementList(InterceptEventArgs e)
        {
            try
            {
                if (SetResult(Achievements.Parse(e.Packet)))
                    e.Block();
            }
            catch (Exception ex) { SetException(ex); }
        }
    }
}
