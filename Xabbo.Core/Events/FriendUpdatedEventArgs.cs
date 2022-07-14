using System;

namespace Xabbo.Core.Events;

public class FriendUpdatedEventArgs : FriendEventArgs
{
    public IFriend Previous { get; }

    public FriendUpdatedEventArgs(IFriend previous, IFriend current)
        : base(current)
    {
        Previous = previous;
    }
}
