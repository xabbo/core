using System;

using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Tasks;

[Intercepts]
public sealed partial class GetAchievementsTask(IInterceptor interceptor)
    : InterceptorTask<IAchievements>(interceptor)
{
    protected override void OnExecute() => Interceptor.Send(Out.GetAchievements);

    [InterceptIn(nameof(In.Achievements))]
    void OnAchievements(Intercept e)
    {
        try
        {
            if (SetResult(e.Packet.Parse<Achievements>()))
                e.Block();
        }
        catch (Exception ex) { SetException(ex); }
    }
}
