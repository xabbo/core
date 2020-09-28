using System;
using System.Collections.Generic;

namespace Xabbo.Core.Events
{
    public class FriendsEventArgs : EventArgs
    {
        public IReadOnlyList<IFriendInfo> Friends { get; }

        public FriendsEventArgs(IEnumerable<IFriendInfo> friends)
        {
            Friends = new List<IFriendInfo>(friends).AsReadOnly();
        }
    }
}
