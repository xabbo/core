using System;
using System.Threading.Tasks;

using Xabbo.Core.Messages;

namespace Xabbo.Core.Tasks
{
    [RequiredOut("RequestUserData")]
    public class GetUserDataTask : InterceptorTask<UserData>
    {
        public GetUserDataTask(IInterceptor interceptor)
            : base(interceptor)
        { }

        protected override Task OnExecuteAsync() => SendAsync(Out.RequestUserData);

        [InterceptIn("UserData")]
        private void OnUserData(InterceptEventArgs e)
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
