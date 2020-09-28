using System;

namespace Xabbo.Core.Events
{
    public class FriendEventArgs : EventArgs
    {
        public IFriendInfo Friend { get; }

        public FriendEventArgs(IFriendInfo friend)
        {
            Friend = friend;
        }
    }
}
