using System;
using System.Threading.Tasks;

using Xabbo.Messages.Flash;
using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;

namespace Xabbo.Core.Tasks;

public class GetProfileTask : InterceptorTask<IUserProfile>
{
    private readonly long _userId;

    public GetProfileTask(IInterceptor interceptor, long userId)
        : base(interceptor)
    {
        _userId = userId;
    }

    protected override void OnExecute() => Interceptor.Send(Out.GetExtendedProfile, _userId, false);

    [InterceptIn(nameof(In.ExtendedProfile))]
    protected void OnUserProfile(Intercept e)
    {
        try
        {
            var userProfile = e.Packet.Parse<UserProfile>();
            if (userProfile.Id == _userId)
            {
                if (SetResult(userProfile))
                    e.Block();
            }
        }
        catch (Exception ex) { SetException(ex); }
    }
}
