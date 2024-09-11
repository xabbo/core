using System;
using System.Collections.Generic;

using Xabbo.Messages.Flash;
using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;

namespace Xabbo.Core.Tasks;

[Intercept]
public sealed partial class GetRightsListTask(IInterceptor interceptor, long roomId)
    : InterceptorTask<List<(long Id, string Name)>>(interceptor)
{
    private readonly long _roomId = roomId;

    protected override void OnExecute() => Interceptor.Send(Out.GetFlatControllers, _roomId);

    [InterceptIn(nameof(In.FlatControllers))]
    private void HandleFlatControllers(Intercept e)
    {
        try
        {
            long roomId = e.Packet.Read<Id>();
            if (roomId == _roomId)
            {
                var list = new List<(long, string)>();
                int n = e.Packet.Read<Length>();
                for (int i = 0; i < n; i++)
                    list.Add((e.Packet.Read<Id>(), e.Packet.Read<string>()));

                if (SetResult(list))
                    e.Block();
            }
        }
        catch (Exception ex) { SetException(ex); }
    }
}
