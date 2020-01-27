using System;
using System.Threading.Tasks;

using Xabbo.Core.Messages;

namespace Xabbo.Core
{
    public class GetGroupDataTask : InterceptorTask<GroupData>
    {
        private readonly int groupId;

        public GetGroupDataTask(IInterceptor interceptor, int groupId)
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
                var groupData = GroupData.Parse(e.Packet);
                if (groupData.Id == groupId)
                {
                    e.Block();
                    SetResult(groupData);
                }
            }
            catch (Exception ex) { SetException(ex); }
        }
    }
}
