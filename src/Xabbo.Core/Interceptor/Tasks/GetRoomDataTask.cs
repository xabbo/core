using System;

using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Tasks;

[Intercept]
public sealed partial class GetRoomDataTask : InterceptorTask<IRoomData>
{
    private readonly long _roomId;

    public GetRoomDataTask(IInterceptor interceptor, long roomId)
        : base(interceptor)
    {
        _roomId = roomId;
    }

    protected override void OnExecute() => Interceptor.Send(Out.GetGuestRoom, _roomId, 0, 0);

    [InterceptIn(nameof(In.GetGuestRoomResult))]
    void OnRoomData(Intercept e)
    {
        try
        {
            var roomData = e.Packet.Read<RoomData>();
            if (roomData.Id == _roomId)
            {
                if (SetResult(roomData))
                    e.Block();
            }
        }
        catch (Exception ex) { SetException(ex); }
    }
}
