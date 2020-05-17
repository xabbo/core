using System;

namespace Xabbo.Core.Events
{
    public class FriendEventArgs : EventArgs
    {
        public FriendInfo Friend { get; }

        public FriendEventArgs(FriendInfo friend)
        {
            Friend = friend;
        }
    }
}
