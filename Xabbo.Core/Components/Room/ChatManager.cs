using System;

using Xabbo.Core.Events;
using Xabbo.Core.Messages;

namespace Xabbo.Core.Components
{
    [Dependencies(typeof(RoomManager), typeof(EntityManager))]
    public class ChatManager : XabboComponent
    {
        private RoomManager roomManager;
        private EntityManager entities;

        public event EventHandler<EntityChatEventArgs>? EntityChat;

        protected virtual void OnEntityChat(EntityChatEventArgs e)
            => EntityChat?.Invoke(this, e);

        protected override void OnInitialize()
        {
            roomManager = GetComponent<RoomManager>();
            entities = GetComponent<EntityManager>();
        }

        [InterceptIn(nameof(Incoming.Whisper), nameof(Incoming.Chat), nameof(Incoming.Shout))]
        private void HandleChat(InterceptArgs e)
        {
            if (!roomManager.IsInRoom) return;

            var packet = e.Packet;

            ChatType chatType;
            if (packet.Header == In.Whisper)
                chatType = ChatType.Whisper;
            else if (packet.Header == In.Chat)
                chatType = ChatType.Talk;
            else if (packet.Header == In.Shout)
                chatType = ChatType.Shout;
            else
                throw new Exception($"Unable to detect chat type from incoming header: {packet.Header}");

            int index = packet.ReadInt();
            var entity = entities.GetEntityByIndex(index);
            if (entity == null)
            {
                DebugUtil.Log($"unable to find entity {index}");
                return;
            }

            string message = packet.ReadString();
            int expression = packet.ReadInt();
            int bubbleStyle = packet.ReadInt();

            // string? int

            var eventArgs = new EntityChatEventArgs(entity, chatType, message, bubbleStyle);
            OnEntityChat(eventArgs);

            if (eventArgs.IsBlocked || entity.IsHidden) e.Block();
        }
    }
}
