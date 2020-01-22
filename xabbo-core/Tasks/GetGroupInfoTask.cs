using System;
using System.Threading.Tasks;

using Xabbo.Core.Messages;

namespace Xabbo.Core
{
    public class GetGroupInfoTask : InterceptorTask<GroupInfo>
    {
        private readonly int groupId;

        public GetGroupInfoTask(IInterceptor interceptor, int groupId)
            : base(interceptor)
        {
            this.groupId = groupId;
        }

        protected override Task OnExecuteAsync() => SendAsync(Out.RequestGuildInfo, groupId, false);

        [InterceptIn("GuildInfo")]
        private void OnGuildInfo(InterceptEventArgs e)
        {
            try
            {
                var groupInfo = GroupInfo.Parse(e.Packet);
                if (groupInfo.Id == groupId)
                {
                    e.Block();
                    SetResult(groupInfo);
                }
            }
            catch (Exception ex) { SetException(ex); }
        }
    }
}
