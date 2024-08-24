using System;
using System.Threading.Tasks;

using Xabbo.Messages.Flash;
using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;

namespace Xabbo.Core.Tasks;

public class GetGroupDataTask : InterceptorTask<IGroupData>
{
    private readonly long _groupId;

    public GetGroupDataTask(IInterceptor interceptor, long groupId)
        : base(interceptor)
    {
        _groupId = groupId;
    }

    protected override void OnExecute() => Interceptor.Send(Out.GetHabboGroupDetails, _groupId, false);

    [InterceptIn(nameof(In.HabboGroupDetails))]
    protected void OnHabboGroupDetails(Intercept e)
    {
        try
        {
            var groupData = e.Packet.Parse<GroupData>();
            if (groupData.Id == _groupId)
            {
                if (SetResult(groupData))
                    e.Block();
            }
        }
        catch (Exception ex) { SetException(ex); }
    }
}
