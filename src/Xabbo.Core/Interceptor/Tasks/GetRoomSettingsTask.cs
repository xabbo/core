using System;
using System.Threading.Tasks;

using Xabbo.Messages.Flash;
using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;

namespace Xabbo.Core.Tasks;

public class GetRoomSettingsTask : InterceptorTask<RoomSettings>
{
    private readonly long _roomId;

    public GetRoomSettingsTask(IInterceptor interceptor, long roomId)
        : base(interceptor)
    {
        _roomId = roomId;
    }

    protected override void OnExecute() => Interceptor.Send(Out.GetRoomSettings, _roomId);

    [InterceptIn(nameof(In.RoomSettingsData))]
    protected void OnRoomSettingsData(Intercept e)
    {
        try
        {
            var roomSettings = e.Packet.Parse<RoomSettings>();
            if (roomSettings.Id == _roomId)
            {
                if (SetResult(roomSettings))
                    e.Block();
            }
        }
        catch (Exception ex) { SetException(ex); }
    }
}
