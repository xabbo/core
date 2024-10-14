using System;
using Microsoft.Extensions.Logging;

using Xabbo.Core.Events;
using Xabbo.Core.Messages.Incoming;

namespace Xabbo.Core.Game;

partial class FriendManager
{
    [Intercept]
    private void HandleMessengerInit(Intercept<MessengerInitMsg> e)
    {
        using var scope = _logger.MethodScope();

        _receivedMessengerInit = true;
        if (_isLoading)
        {
            if (_isForceLoading)
                e.Block();

            if (Session.Is(ClientType.Origins))
            {
                // Friends list is available here.
                AddFriends(e.Msg.Friends);
                CompleteLoadingFriends();
            }
        }
    }

    [Intercept]
    private void HandleFriendList(Intercept<FriendListMsg> e)
    {
        if (Session.Is(ClientType.Origins))
        {
            UpdateFriendsOrigins(e.Msg);
            return;
        }

        if (!_isLoading)
            return;

        using var scope = _logger.MethodScope();

        int total = e.Msg.FragmentCount;
        int current = e.Msg.FragmentIndex;

        if (current != _currentFragment)
        {
            _logger.LogWarning("Fragment index mismatch. (expected: {Expected}, actual: {Actual})",
                _currentFragment, current);
            return;
        }
        if (_totalFragments == -1) _totalFragments = total;
        else if (_totalFragments != total)
        {
            _logger.LogWarning("Fragment count mismatch. (expected: {Expected}, actual: {Actual})",
                _totalFragments, total);
            return;
        }

        _logger.LogTrace(
            "Received friend list fragment {FragmentIndex}/{TotalFragments} ({FragmentCount})",
            _currentFragment, total, e.Msg.Count
        );

        if (_isForceLoading)
        {
            _logger.LogTrace("Blocking packet.");
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
    private void HandleFriendsUpdated(FriendsUpdatedMsg msg)
    {
        using var scope = _logger.MethodScope();

        _logger.LogDebug("Processing {Count} friend update(s).", msg.Updates.Count);

        foreach (var update in msg.Updates)
        {
            switch (update.Type)
            {
                case FriendListUpdateType.Add:
                    if (update.Friend is null)
                    {
                        _logger.LogWarning("Friend #{Id} was null when attempting to add.", update.Id);
                        continue;
                    }
                    AddFriend(update.Friend);
                    break;
                case FriendListUpdateType.Update:
                    if (update.Friend is null)
                    {
                        _logger.LogWarning("Friend #{Id} was null when attempting to update.", update.Id);
                        continue;
                    }
                    UpdateFriend(update.Friend);
                    break;
                case FriendListUpdateType.Remove:
                    RemoveFriend(update.Id);
                    break;
            }
        }
    }

    [Intercept]
    private void HandleFriendAdded(FriendAddedMsg msg)
    {
        using (_logger.MethodScope())
            AddFriend(msg.Friend);
    }

    [Intercept]
    private void HandleFriendsRemoved(FriendsRemovedMsg msg)
    {
        using (_logger.MethodScope())
        {
            foreach (var id in msg)
                RemoveFriend(id);
        }
    }

    [Intercept]
    private void HandleFriendRequest(FriendRequestMsg request)
    {
        using (_logger.MethodScope())
            ReceiveFriendRequest(request.Id, request.Name, request.FigureString);
    }

    [Intercept]
    private void HandleConsoleMessage(ConsoleMessageMsg msg)
    {
        using (_logger.MethodScope())
            ReceiveMessage(msg.Message);
    }

    [Intercept]
    private void HandleConsoleMessages(ConsoleMessagesMsg messages)
    {
        using (_logger.MethodScope())
            ReceiveMessages(messages);
    }
}