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
        private readonly long _roomId;

        public GetRightsListTask(IInterceptor interceptor, long roomId)
            : base(interceptor)
        {
            _roomId = roomId;
        }

        protected override ValueTask OnExecuteAsync() => Interceptor.SendAsync(Out.GetFlatControllers, (LegacyLong)_roomId);

        [InterceptIn(nameof(Incoming.FlatControllers))]
        protected void OnRoomRightsList(InterceptArgs e)
        {
            try
            {
                long roomId = e.Packet.ReadLegacyLong();
                if (roomId == _roomId)
                {
                    var list = new List<(long, string)>();
                    short n = e.Packet.ReadLegacyShort();
                    for (int i = 0; i < n; i++)
                        list.Add((e.Packet.ReadLegacyLong(), e.Packet.ReadString()));

                    if (SetResult(list))
                        e.Block();
                }
            }
            catch (Exception ex) { SetException(ex); }
        }
    }
}
