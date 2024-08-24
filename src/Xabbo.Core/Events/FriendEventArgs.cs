using System;

namespace Xabbo.Core.Events;

public class FriendEventArgs(IFriend friend)
{
    public IFriend Friend { get; } = friend;
}
