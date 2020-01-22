using System;
using System.Threading.Tasks;

using Xabbo.Core.Messages;

namespace Xabbo.Core
{
    public class GetRoomDataTask : InterceptorTask<RoomData>
    {
        private readonly int roomId;

        public GetRoomDataTask(IInterceptor interceptor, int roomId)
            : base(interceptor)
        {
            this.roomId = roomId;
        }

        protected override Task OnExecuteAsync() => SendAsync(Out.RequestRoomData, roomId, 0, 0);

        [InterceptIn("RoomData")]
        private void OnRoomData(InterceptEventArgs e)
        {
            try
            {
                var roomData = RoomData.Parse(e.Packet);
                if (roomData.Id == roomId)
                {
                    e.Block();
                    SetResult(roomData);
                }
            }
            catch (Exception ex) { SetException(ex); }
        }
    }
}
