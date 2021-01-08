using System;
using System.Threading.Tasks;

using Xabbo.Core.Messages;

namespace Xabbo.Core.Tasks
{
    // @Update [RequiredOut(nameof(Outgoing.RequestGuildInfo))]
    public class GetGroupDataTask : InterceptorTask<IGroupData>
    {
        private readonly int groupId;

        public GetGroupDataTask(IInterceptor interceptor, int groupId)
            : base(interceptor)
        {
            this.groupId = groupId;
        }

        protected override Task OnExecuteAsync() => throw new NotImplementedException(); // @Update SendAsync(Out.RequestGuildInfo, groupId, false);

        // @Update [InterceptIn(nameof(Incoming.GuildInfo))]
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
