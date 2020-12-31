﻿using System;
using System.Threading.Tasks;

using Xabbo.Core.Messages;

namespace Xabbo.Core.Tasks
{
    public class GetRoomSettingsTask : InterceptorTask<RoomSettings>
    {
        private readonly long _roomId;

        public GetRoomSettingsTask(IInterceptor interceptor, long roomId)
            : base(interceptor)
        {
            _roomId = roomId;
        }

        protected override Task OnExecuteAsync() => SendAsync(Out.GetRoomSettings, _roomId);

        [InterceptIn(nameof(Incoming.RoomSettingsData))]
        protected void OnRoomSettingsData(InterceptArgs e)
        {
            try
            {
                var roomSettings = RoomSettings.Parse(e.Packet);
                if (roomSettings.Id == _roomId)
                {
                    if (SetResult(roomSettings))
                        e.Block();
                }
            }
            catch (Exception ex) { SetException(ex); }
        }
    }
}
