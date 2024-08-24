using System;
using System.Threading.Tasks;

using Xabbo.Messages.Flash;
using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;

namespace Xabbo.Core.Tasks;

public class GetUserDataTask : InterceptorTask<IUserData>
{
    public GetUserDataTask(IInterceptor interceptor)
        : base(interceptor)
    { }

    protected override void OnExecute() => Interceptor.Send(Out.InfoRetrieve);

    [InterceptIn(nameof(In.UserObject))]
    protected void HandleUserData(Intercept e)
    {
        try
        {
            if (SetResult(e.Packet.Parse<UserData>()))
                e.Block();
        }
        catch (Exception ex) { SetException(ex); }
    }
}
