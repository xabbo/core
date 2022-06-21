using System;
using System.Threading.Tasks;

using Xabbo.Messages;
using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;
using Xabbo.Interceptor.Attributes;

namespace Xabbo.Core.Tasks
{
    public class GetGroupDataTask : InterceptorTask<IGroupData>
    {
        private readonly long _groupId;

        public GetGroupDataTask(IInterceptor interceptor, long groupId)
            : base(interceptor)
        {
            _groupId = groupId;
        }

        protected override ValueTask OnExecuteAsync() => Interceptor.SendAsync(Out.GetHabboGroupDetails, (LegacyLong)_groupId, false);

        [InterceptIn(nameof(Incoming.HabboGroupDetails))]
        protected void OnHabboGroupDetails(InterceptArgs e)
        {
            try
            {
                var groupData = GroupData.Parse(e.Packet);
                if (groupData.Id == _groupId)
                {
                    if (SetResult(groupData))
                        e.Block();
                }
            }
            catch (Exception ex) { SetException(ex); }
        }
    }
}
