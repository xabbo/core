using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xabbo.Core.Messages;

namespace Xabbo.Core.Tasks
{
    [RequiredOut(nameof(Outgoing.RequestRoomRights))]
    public class GetRightsListTask : InterceptorTask<List<(int Id, string Name)>>
    {
        private readonly int roomId;

        public GetRightsListTask(IInterceptor interceptor, int roomId)
            : base(interceptor)
        {
            this.roomId = roomId;
        }

        protected override Task OnExecuteAsync() => SendAsync(Out.RequestRoomRights, roomId);

        [InterceptIn(nameof(Incoming.RoomRightsList))]
        protected void OnRoomRightsList(InterceptArgs e)
        {
            try
            {
                int roomId = e.Packet.ReadInt();
                if (roomId == this.roomId)
                {
                    var list = new List<(int, string)>();
                    int n = e.Packet.ReadInt();
                    for (int i = 0; i < n; i++)
                        list.Add((e.Packet.ReadInt(), e.Packet.ReadString()));

                    if (SetResult(list))
                        e.Block();
                }
            }
            catch (Exception ex) { SetException(ex); }
        }
    }
}
