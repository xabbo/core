using System;

using Xabbo.Core.Events;
using Xabbo.Core.Messages;

namespace Xabbo.Core.Game
{
    public class ChatManager : GameStateManager
    {
        private readonly RoomManager _room;
        private readonly EntityManager _entities;

        public event EventHandler<EntityChatEventArgs>? EntityChat;

        protected virtual void OnEntityChat(EntityChatEventArgs e)
            => EntityChat?.Invoke(this, e);

        public ChatManager(IInterceptor interceptor,
            RoomManager roomManager, EntityManager entityManager)
            : base(interceptor)
        {
            _room = roomManager;
            _entities = entityManager;
        }

        [InterceptIn(nameof(Incoming.Whisper), nameof(Incoming.Chat), nameof(Incoming.Shout))]
        private void HandleChat(InterceptArgs e)
        {
            if (!_room.IsInRoom) return;

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
            var entity = _entities.GetEntityByIndex(index);
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
