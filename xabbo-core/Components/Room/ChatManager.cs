using System;

using Xabbo.Core.Events;
using Xabbo.Core.Messages;
using Xabbo.Core.Protocol;

namespace Xabbo.Core.Components
{
    [Dependencies(typeof(RoomManager), typeof(EntityManager))]
    public class ChatManager : XabboComponent
    {
        private EntityManager entities;

        public event EventHandler<EntityChatEventArgs> EntityChat;

        protected virtual void OnEntityChat(Entity entity, ChatType chatType, string message, int bubbleStyle)
            => EntityChat?.Invoke(this, new EntityChatEventArgs(entity, chatType, message, bubbleStyle));

        protected override void OnInitialize()
        {
            entities = GetComponent<EntityManager>();
        }

        [Receive("RoomUserWhisper", "RoomUserTalk", "RoomUserShout")]
        private void HandleEntityChat(Packet packet)
        {
            ChatType chatType;
            if (packet.Header == In.RoomUserWhisper)
                chatType = ChatType.Whisper;
            else if (packet.Header == In.RoomUserTalk)
                chatType = ChatType.Talk;
            else if (packet.Header == In.RoomUserShout)
                chatType = ChatType.Shout;
            else
                throw new Exception($"Unable to detect chat type from incoming header: {packet.Header}");

            int index = packet.ReadInteger();
            var entity = entities.GetEntity(index);

            string message = packet.ReadString();
            packet.ReadInteger();
            int bubbleStyle = packet.ReadInteger();

            OnEntityChat(entity, chatType, message, bubbleStyle);
        }
    }
}
