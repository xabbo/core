using System;
using System.Threading.Tasks;

using Xabbo.Core.Messages;

namespace Xabbo.Core.Tasks
{
    // @Update [RequiredOut(nameof(Outgoing.RequestAchievements))]
    public class GetAchievementsTask : InterceptorTask<IAchievements>
    {
        public GetAchievementsTask(IInterceptor interceptor)
            : base(interceptor)
        { }

        protected override Task OnExecuteAsync() => throw new NotImplementedException(); // @Update SendAsync(Out.RequestAchievements);

        // @Update [InterceptIn(nameof(Incoming.AchievementList))]
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
