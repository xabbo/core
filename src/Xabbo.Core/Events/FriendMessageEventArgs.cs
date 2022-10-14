using System;
using System.Threading.Tasks;

using Xabbo.Core.Game;

namespace Xabbo.Core.Events;

public class FriendMessageEventArgs : FriendEventArgs
{
    private readonly FriendManager _friendManager;

    public string Message { get; }

    public FriendMessageEventArgs(FriendManager friendManager,
        IFriend friend, string message)
        : base(friend)
    {
        _friendManager = friendManager;
        Message = message;
    }

    public void Reply(string message) => _friendManager.SendMessage(Friend.Id, message);
    public ValueTask ReplyAsync(string message) => _friendManager.SendMessageAsync(Friend.Id, message);
}
