using System;

using Xabbo.Core.Game;

namespace Xabbo.Core.Events
{
    public class FriendMessageEventArgs : FriendEventArgs
    {
        private readonly FriendManager friendManager;

        public string Message { get; }

        public FriendMessageEventArgs(FriendManager friendManager,
            IFriend friend, string message)
            : base(friend)
        {
            this.friendManager = friendManager;
            Message = message;
        }

        public void Reply(string message) => friendManager.SendMessage(Friend.Id, message);
    }
}
