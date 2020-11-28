using System;
using System.Threading.Tasks;

using Xabbo.Core.Messages;

namespace Xabbo.Core.Tasks
{
    [RequiredOut("RequestRoomData", "RequestRoomLoad")]
    public class LoadRoomTask : InterceptorTask<bool>
    {
        private readonly int roomId;
        private readonly string password;

        public LoadRoomTask(IInterceptor interceptor, int roomId, string password = "")
            : base(interceptor)
        {
            throw new NotImplementedException();

            this.roomId = roomId;
            this.password = password;
        }

        protected override Task OnExecuteAsync() => SendAsync(Out.RequestRoomData, roomId, 0, 1);

        [InterceptIn("RoomData")]
        private void HandleRoomData(InterceptArgs e)
        {
            try
            {
                var roomData = RoomData.Parse(e.Packet);

            }
            catch (Exception ex) { SetException(ex); }
        }
    }
}
