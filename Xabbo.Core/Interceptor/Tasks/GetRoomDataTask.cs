﻿using System;
using System.Threading.Tasks;

using Xabbo.Core.Messages;

namespace Xabbo.Core.Tasks
{
    public class GetRoomDataTask : InterceptorTask<IRoomData>
    {
        private readonly long _roomId;

        public GetRoomDataTask(IInterceptor interceptor, long roomId)
            : base(interceptor)
        {
            _roomId = roomId;
        }

        protected override Task OnExecuteAsync() => SendAsync(Out.GetGuestRoom, _roomId, 0, 0);

        [InterceptIn(nameof(Incoming.GetGuestRoomResult))]
        protected void OnRoomData(InterceptArgs e)
        {
            try
            {
                var roomData = RoomData.Parse(e.Packet);
                if (roomData.Id == _roomId)
                {
                    if (SetResult(roomData))
                        e.Block();
                }
            }
            catch (Exception ex) { SetException(ex); }
        }
    }
}
