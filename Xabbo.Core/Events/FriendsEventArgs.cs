using System;
using System.Collections.Generic;

namespace Xabbo.Core.Events;

public class FriendsEventArgs : EventArgs
{
    public IReadOnlyList<IFriend> Friends { get; }

    public FriendsEventArgs(IEnumerable<IFriend> friends)
    {
        Friends = new List<IFriend>(friends).AsReadOnly();
    }
}
