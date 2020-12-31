using System;
using System.Threading.Tasks;

using Xabbo.Core.Messages;

namespace Xabbo.Core.Tasks
{
    public class GetUserDataTask : InterceptorTask<IUserData>
    {
        public GetUserDataTask(IInterceptor interceptor)
            : base(interceptor)
        { }

        protected override Task OnExecuteAsync() => SendAsync(Out.InfoRetrieve);

        [InterceptIn(nameof(Incoming.UserObject))]
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
