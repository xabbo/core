using System;
using System.Threading.Tasks;

using Xabbo.Messages;
using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;

namespace Xabbo.Core.Tasks;

public class GetStickyTask : InterceptorTask<Sticky>
{
    private readonly long _stickyId;

    public GetStickyTask(IInterceptor interceptor, long stickyId)
        : base(interceptor)
    {
        _stickyId = stickyId;
    }

    protected override ValueTask OnExecuteAsync() => Interceptor.SendAsync(Out.GetItemData, _stickyId);

    [InterceptIn(nameof(Incoming.ItemData))]
    protected void OnPostItData(InterceptArgs e)
    {
        try
        {
            var sticky = Sticky.Parse(e.Packet);
            if (sticky.Id == _stickyId)
            {
                if (SetResult(sticky))
                    e.Block();
            }
        }
        catch (Exception ex) { SetException(ex); }
    }
}
