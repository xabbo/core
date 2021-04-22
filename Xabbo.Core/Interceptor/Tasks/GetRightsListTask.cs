using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xabbo.Messages;
using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;

namespace Xabbo.Core.Tasks
{
    public class GetRightsListTask : InterceptorTask<List<(long Id, string Name)>>
    {
        private readonly long roomId;

        public GetRightsListTask(IInterceptor interceptor, long _roomId)
            : base(interceptor)
        {
            this.roomId = _roomId;
        }

        protected override Task OnExecuteAsync() => SendAsync(Out.GetFlatControllers, roomId);

        [InterceptIn(nameof(Incoming.FlatControllers))]
        protected void OnRoomRightsList(InterceptArgs e)
        {
            try
            {
                long roomId = e.Packet.ReadLong();
                if (roomId == this.roomId)
                {
                    var list = new List<(long, string)>();
                    short n = e.Packet.ReadShort();
                    for (int i = 0; i < n; i++)
                        list.Add((e.Packet.ReadLong(), e.Packet.ReadString()));

                    if (SetResult(list))
                        e.Block();
                }
            }
            catch (Exception ex) { SetException(ex); }
        }
    }
}
