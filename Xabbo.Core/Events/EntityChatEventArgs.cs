using System;

namespace Xabbo.Core.Events
{
    public class EntityChatEventArgs : EntityEventArgs
    {
        public ChatType ChatType { get; }
        public string Message { get; }
        public int BubbleStyle { get; }

        public bool IsBlocked { get; private set; }

        public EntityChatEventArgs(IEntity entity, ChatType chatType, string message, int bubbleStyle)
            : base(entity)
        {
            ChatType = chatType;
            Message = message;
            BubbleStyle = bubbleStyle;
        }

        public void Block() { IsBlocked = true; }
    }
}
