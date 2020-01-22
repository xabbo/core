﻿using System;
using System.Threading.Tasks;

using Xabbo.Core.Messages;

namespace Xabbo.Core
{
    internal class GetRoomSettingsTask : InterceptorTask<RoomSettings>
    {
        private readonly int roomId;

        public GetRoomSettingsTask(IInterceptor interceptor, int roomId)
            : base(interceptor)
        {
            this.roomId = roomId;
        }

        protected override Task OnExecuteAsync() => SendAsync(Out.RequestRoomSettings, roomId);

        [InterceptIn("RoomSettings")]
        private void OnRoomSettings(InterceptEventArgs e)
        {
            try
            {
                var roomSettings = RoomSettings.Parse(e.Packet);
                if (roomSettings.Id == roomId)
                {
                    e.Block();
                    SetResult(roomSettings);
                }
            }
            catch (Exception ex) { SetException(ex); }
        }
    }
}
