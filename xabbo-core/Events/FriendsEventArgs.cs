using System;
using System.Collections.Generic;

namespace Xabbo.Core.Events
{
    public class FriendsEventArgs : EventArgs
    {
        public IReadOnlyList<FriendInfo> Friends { get; }

        public FriendsEventArgs(IEnumerable<FriendInfo> friends)
        {
            Friends = new List<FriendInfo>(friends).AsReadOnly();
        }
    }
}
