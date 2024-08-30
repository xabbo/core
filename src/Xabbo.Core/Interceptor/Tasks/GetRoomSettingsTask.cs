using System;

using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Tasks;

public class GetRoomSettingsTask(IInterceptor interceptor, Id roomId)
    : InterceptorTask<RoomSettings>(interceptor)
{
    private readonly Id _roomId = roomId;

    protected override void OnExecute() => Interceptor.Send(Out.GetRoomSettings, _roomId);

    [InterceptIn(nameof(In.RoomSettingsData))]
    protected void OnRoomSettingsData(Intercept e)
    {
        try
        {
            var roomSettings = e.Packet.Read<RoomSettings>();
            if (roomSettings.Id == _roomId)
            {
                if (SetResult(roomSettings))
                    e.Block();
            }
        }
        catch (Exception ex) { SetException(ex); }
    }
}
