using System;

using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Tasks;

[Intercepts]
public partial class GetStickyTask(IInterceptor interceptor, Id stickyId) : InterceptorTask<Sticky>(interceptor)
{
    private readonly Id _stickyId = stickyId;

    protected override void OnExecute() => Interceptor.Send(Out.GetItemData, _stickyId);

    [InterceptIn(nameof(In.RequestSpamWallPostIt))] // TODO: check this
    protected void OnPostItData(Intercept e)
    {
        try
        {
            var sticky = e.Packet.Parse<Sticky>();
            if (sticky.Id == _stickyId)
            {
                if (SetResult(sticky))
                    e.Block();
            }
        }
        catch (Exception ex) { SetException(ex); }
    }
}
