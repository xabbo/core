using System;
using System.Threading.Tasks;

using Xabbo.Core.Messages;

namespace Xabbo.Core.Tasks
{
    [RequiredOut(nameof(Outgoing.RequestUserData))]
    public class GetUserDataTask : InterceptorTask<IUserData>
    {
        public GetUserDataTask(IInterceptor interceptor)
            : base(interceptor)
        { }

        protected override Task OnExecuteAsync() => SendAsync(Out.RequestUserData);

        [InterceptIn(nameof(Incoming.UserData))]
        protected void HandleUserData(InterceptArgs e)
        {
            try
            {
                if (SetResult(UserData.Parse(e.Packet)))
                    e.Block();
            }
            catch (Exception ex) { SetException(ex); }
        }
    }
}
