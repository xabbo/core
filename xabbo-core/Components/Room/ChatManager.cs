using System;

using Xabbo.Core.Events;
using Xabbo.Core.Messages;
using Xabbo.Core.Protocol;

namespace Xabbo.Core.Components
{
    [Dependencies(typeof(EntityManager))]
    public class ChatManager : XabboComponent
    {
        private EntityManager entities;

        public event EventHandler<EntityChatEventArgs> EntityChat;

        protected virtual void OnEntityChat(EntityChatEventArgs e)
            => EntityChat?.Invoke(this, e);

        protected override void OnInitialize()
        {
            entities = GetComponent<EntityManager>();
        }

        [InterceptIn("RoomUserWhisper", "RoomUserTalk", "RoomUserShout")]
        private void HandleEntityChat(InterceptEventArgs e)
        {
            var packet = e.Packet;

            ChatType chatType;
            if (packet.Header == In.RoomUserWhisper)
                chatType = ChatType.Whisper;
            else if (packet.Header == In.RoomUserTalk)
                chatType = ChatType.Talk;
            else if (packet.Header == In.RoomUserShout)
                chatType = ChatType.Shout;
            else
                throw new Exception($"Unable to detect chat type from incoming header: {packet.Header}");

            int index = packet.ReadInt();
            var entity = entities.GetEntityByIndex(index);

            string message = packet.ReadString();
            packet.ReadInt();
            int bubbleStyle = packet.ReadInt();

            var eventArgs = new EntityChatEventArgs(entity, chatType, message, bubbleStyle);
            OnEntityChat(eventArgs);

            if (eventArgs.IsBlocked) e.Block();
        }
    }
}
