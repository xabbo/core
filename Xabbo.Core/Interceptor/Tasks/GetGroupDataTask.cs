using System;
using System.Threading.Tasks;

using Xabbo.Messages;
using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;

namespace Xabbo.Core.Tasks
{
    // @Update [RequiredOut(nameof(Outgoing.RequestGuildInfo))]
    public class GetGroupDataTask : InterceptorTask<IGroupData>
    {
        private readonly long groupId;

        public GetGroupDataTask(IInterceptor interceptor, long groupId)
            : base(interceptor)
        {
            this.groupId = groupId;
        }

        protected override async Task OnExecuteAsync() => await SendAsync(Out.GetHabboGroupDetails, (LegacyLong)groupId, false);

        [InterceptIn(nameof(Incoming.HabboGroupDetails))]
        protected void OnHabboGroupDetails(InterceptArgs e)
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
