using System;
using System.Threading.Tasks;

using Xabbo.Messages;
using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;

namespace Xabbo.Core.Tasks
{
    // @Update [RequiredOut("RequestRoomData", "RequestRoomLoad")]
    public class LoadRoomTask : InterceptorTask<bool>
    {
        private readonly long _roomId;
        private readonly string _password;

        public LoadRoomTask(IInterceptor interceptor, long roomId, string password = "")
            : base(interceptor)
        {
            throw new NotImplementedException();

            _roomId = roomId;
            _password = password;
        }

        protected override Task OnExecuteAsync() => throw new NotImplementedException(); // @Update SendAsync(Out.RequestRoomData, roomId, 0, 1);

        // @Update [InterceptIn("RoomData")]
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
