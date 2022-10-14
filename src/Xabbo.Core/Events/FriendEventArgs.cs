using System;

namespace Xabbo.Core.Events;

public class FriendEventArgs : EventArgs
{
    public IFriend Friend { get; }

    public FriendEventArgs(IFriend friend)
    {
        Friend = friend;
    }
}
