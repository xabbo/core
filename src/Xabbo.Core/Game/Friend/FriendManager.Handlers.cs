using Microsoft.Extensions.Logging;
using Xabbo.Core.Events;

using Xabbo.Core.Messages.Incoming;

namespace Xabbo.Core.Game;

using ModernIncoming = Messages.Incoming;
using OriginsIncoming = Messages.Incoming;

partial class FriendManager
{
    [Intercept]
    private void HandleMessengerInit(Intercept<MessengerInitMsg> e)
    {
        using var scope = Log.MethodScope();

        _receivedMessengerInit = true;
        if (_isLoading)
        {
            if (_isForceLoading)
                e.Block();

            if (Interceptor.Session.Client.Type is ClientType.Shockwave)
            {
                // Friends list is available here
                AddFriends(e.Msg.Friends);
                if (_isLoading)
                    CompleteLoadingFriends();
            }
        }
    }

    [Intercept]
    private void HandleFriendList(Intercept<FriendListMsg> e)
    {
        if (!_isLoading)
            return;

        using var scope = Log.MethodScope();

        int total = e.Msg.FragmentCount;
        int current = e.Msg.FragmentIndex;

        if (current != _currentFragment)
        {
            Log.LogWarning("Fragment index mismatch. (expected: {Expected}, actual: {Actual})",
                _currentFragment, current);
            return;
        }
        if (_totalFragments == -1) _totalFragments = total;
        else if (_totalFragments != total)
        {
            Log.LogWarning("Fragment count mismatch. (expected: {Expected}, actual: {Actual})",
                _totalFragments, total);
            return;
        }

        Log.LogTrace(
            "Received friend list fragment {FragmentIndex}/{TotalFragments} ({FragmentCount})",
            _currentFragment, total, e.Msg.Count
        );

        if (_isForceLoading)
        {
            Log.LogTrace("Blocking packet.");
            e.Block();
        }

        _loadList.AddRange(e.Msg);

        _currentFragment++;
        if (_currentFragment == total)
        {
            AddFriends(_loadList);
            CompleteLoadingFriends();
        }
    }

    [Intercept]
    private void HandleFriendListUpdate(ModernIncoming.FriendListUpdateMsg msg)
    {
        using var scope = Log.MethodScope();

        foreach (var update in msg.Updates)
        {
            switch (update.Type)
            {
                case FriendListUpdateType.Add when update.Friend is not null:
                    AddFriend(update.Friend);
                    break;
                case FriendListUpdateType.Update when update.Friend is not null:
                    UpdateFriend(update.Friend);
                    break;
                case FriendListUpdateType.Remove:
                    RemoveFriend(update.Id);
                    break;
            }
        }
    }

    [Intercept]
    private void HandleFriendAdded(OriginsIncoming.FriendAddedMsg msg)
    {
        using (Log.MethodScope())
            AddFriend(msg.Friend);
    }

    [Intercept]
    private void HandleFriendsRemoved(OriginsIncoming.FriendsRemovedMsg msg)
    {
        using (Log.MethodScope())
        {
            foreach (var id in msg)
                RemoveFriend(id);
        }
    }

    [Intercept]
    private void HandleConsoleMessages(ConsoleMessagesMsg messages)
    {
        using var scope = Log.MethodScope();

        foreach (var message in messages)
        {
            if (!_friends.TryGetValue(message.SenderId, out Friend? friend))
            {
                Log.LogWarning("Failed to get friend #{Id} from ID map.", message.SenderId);
                continue;
            }

            Log.LogInformation("Received message from '{Name}': '{Message}'.", friend.Name, message.Content);
            MessageReceived?.Invoke(new FriendMessageEventArgs(friend, message));
        }
    }
}