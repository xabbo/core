using System;
using System.Threading.Tasks;

using Xabbo.Core.Messages;

namespace Xabbo.Core.Tasks
{
    [RequiredOut(nameof(Outgoing.RequestGuildInfo))]
    public class GetGroupDataTask : InterceptorTask<IGroupData>
    {
        private readonly int groupId;

        public GetGroupDataTask(IInterceptor interceptor, int groupId)
            : base(interceptor)
        {
            this.groupId = groupId;
        }

        protected override Task OnExecuteAsync() => SendAsync(Out.RequestGuildInfo, groupId, false);

        [InterceptIn(nameof(Incoming.GuildInfo))]
        protected void OnGuildInfo(InterceptArgs e)
        {
            try
            {
                var groupData = GroupData.Parse(e.Packet);
                if (groupData.Id == groupId)
                {
                    if (SetResult(groupData))
                        e.Block();
                }
            }
            catch (Exception ex) { SetException(ex); }
        }
    }
}
