using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xabbo.Core.Messages;

namespace Xabbo.Core
{
    internal class GetRightsListTask : InterceptorTask<List<(int Id, string Name)>>
    {
        private readonly int roomId;

        public GetRightsListTask(IInterceptor interceptor, int roomId)
            : base(interceptor)
        {
            this.roomId = roomId;
        }

        protected override Task OnExecuteAsync() => SendAsync(Out.RequestRoomRights, roomId);

        [InterceptIn("RoomRightsList")]
        private void OnRoomRightsList(InterceptEventArgs e)
        {
            try
            {
                int roomId = e.Packet.ReadInteger();
                if (roomId == this.roomId)
                {
                    e.Block();

                    var list = new List<(int, string)>();
                    int n = e.Packet.ReadInteger();
                    for (int i = 0; i < n; i++)
                        list.Add((e.Packet.ReadInteger(), e.Packet.ReadString()));

                    SetResult(list);
                }
            }
            catch (Exception ex) { SetException(ex); }
        }
    }
}
