using System;
using System.Collections.Generic;

using Xabbo.Messages.Flash;
using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;

namespace Xabbo.Core.Tasks;

[Intercept(~ClientType.Shockwave)]
public sealed partial class GetRightsListTask(IInterceptor interceptor, Id roomId)
    : InterceptorTask<List<(long Id, string Name)>>(interceptor)
{
    private readonly Id _roomId = roomId;

    protected override ClientType SupportedClients => ~ClientType.Shockwave;

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
