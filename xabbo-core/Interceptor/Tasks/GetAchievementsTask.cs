using System;
using System.Threading.Tasks;

using Xabbo.Core.Messages;

namespace Xabbo.Core.Tasks
{
    [RequiredOut(nameof(Outgoing.RequestAchievements))]
    public class GetAchievementsTask : InterceptorTask<IAchievements>
    {
        public GetAchievementsTask(IInterceptor interceptor)
            : base(interceptor)
        { }

        protected override Task OnExecuteAsync() => SendAsync(Out.RequestAchievements);

        [InterceptIn(nameof(Incoming.AchievementList))]
        protected void OnAchievementList(InterceptArgs e)
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
