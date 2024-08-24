using System.Collections.Generic;

namespace Xabbo.Core.Events;

public sealed class FriendsEventArgs(IEnumerable<IFriend> friends)
{
    public IReadOnlyList<IFriend> Friends { get; } = new List<IFriend>(friends).AsReadOnly();
}
