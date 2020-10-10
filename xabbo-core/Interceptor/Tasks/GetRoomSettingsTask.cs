using System;
using System.Threading.Tasks;

using Xabbo.Core.Messages;

namespace Xabbo.Core.Tasks
{
    [RequiredOut(nameof(Outgoing.RequestRoomSettings))]
    public class GetRoomSettingsTask : InterceptorTask<RoomSettings>
    {
        private readonly int roomId;

        public GetRoomSettingsTask(IInterceptor interceptor, int roomId)
            : base(interceptor)
        {
            this.roomId = roomId;
        }

        protected override Task OnExecuteAsync() => SendAsync(Out.RequestRoomSettings, roomId);

        [InterceptIn(nameof(Incoming.RoomSettings))]
        protected void OnRoomSettings(InterceptEventArgs e)
        {
            try
            {
                var roomSettings = RoomSettings.Parse(e.Packet);
                if (roomSettings.Id == roomId)
                {
                    if (SetResult(roomSettings))
                        e.Block();
                }
            }
            catch (Exception ex) { SetException(ex); }
        }
    }
}
