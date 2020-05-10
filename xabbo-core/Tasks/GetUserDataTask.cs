using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xabbo.Core.Messages;

namespace Xabbo.Core
{
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
                e.Block();
                SetResult(UserData.Parse(e.Packet));
            }
            catch (Exception ex) { SetException(ex); }
        }
    }
}
