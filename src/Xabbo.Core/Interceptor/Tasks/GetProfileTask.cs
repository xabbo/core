using System;

using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Tasks;

public class GetProfileTask(IInterceptor interceptor, Id userId)
    : InterceptorTask<IUserProfile>(interceptor)
{
    private readonly Id _userId = userId;

    protected override void OnExecute() => Interceptor.Send(Out.GetExtendedProfile, _userId, false);

    [InterceptIn(nameof(In.ExtendedProfile))]
    protected void OnUserProfile(Intercept e)
    {
        try
        {
            var userProfile = e.Packet.Read<UserProfile>();
            if (userProfile.Id == _userId)
            {
                if (SetResult(userProfile))
                    e.Block();
            }
        }
        catch (Exception ex) { SetException(ex); }
    }
}
