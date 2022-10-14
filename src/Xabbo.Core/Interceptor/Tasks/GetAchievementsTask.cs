using System;
using System.Threading.Tasks;

using Xabbo.Messages;
using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;

namespace Xabbo.Core.Tasks;

public class GetAchievementsTask : InterceptorTask<IAchievements>
{
    public GetAchievementsTask(IInterceptor interceptor)
        : base(interceptor)
    { }

    protected override ValueTask OnExecuteAsync() => Interceptor.SendAsync(Out.GetUserAchievements);

    [InterceptIn(nameof(Incoming.PossibleUserAchievements))]
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
