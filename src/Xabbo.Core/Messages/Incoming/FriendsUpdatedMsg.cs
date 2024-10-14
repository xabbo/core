using System;
using System.Collections.Generic;
using System.Linq;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Received when friends are added, updated, or removed.
/// <para/>
/// Supported clients: <see cref="ClientType.All"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="In.FriendListUpdate"/></item>
/// <item>Shockwave: <see cref="Xabbo.Messages.Shockwave.In.FRIEND_LIST_UPDATE"/></item>
/// </list>
/// </summary>
/// <remarks>
/// On Origins, this message will only contain friend updates.
/// For friends that are added or removed, see <see cref="FriendAddedMsg"/> or
/// <see cref="FriendsRemovedMsg"/>.
/// </remarks>
public sealed class FriendsUpdatedMsg : IMessage<FriendsUpdatedMsg>
{
    static Identifier IMessage<FriendsUpdatedMsg>.Identifier => In.FriendListUpdate;

    /// <summary>
    /// The friend list categories.
    /// </summary>
    /// <remarks>
    /// Used on modern clients.
    /// </remarks>
    public List<FriendCategory> Categories { get; set; } = [];

    /// <summary>
    /// The list of friend updates.
    /// </summary>
    /// <remarks>
    /// Shockwave only supports updates with type <see cref="FriendListUpdateType.Update"/>.
    /// </remarks>
    public List<FriendUpdate> Updates { get; set; } = [];

    static FriendsUpdatedMsg IParser<FriendsUpdatedMsg>.Parse(in PacketReader p)
    {
        if (p.Client is ClientType.Shockwave)
        {
            return new()
            {
                Updates = [.. p.ParseArray<Friend>()
                    .Select(friend => new FriendUpdate(FriendListUpdateType.Update, friend.Id, friend))
                ]
            };
        }
        else
        {
            return new()
            {
                Categories = [.. p.ParseArray<FriendCategory>()],
                Updates = [.. p.ParseArray<FriendUpdate>()],
            };
        }
    }

    void IComposer.Compose(in PacketWriter p)
    {
        if (p.Client is ClientType.Origins)
        {
            if (Updates.Any(update => update.Type is not FriendListUpdateType.Update))
                throw new Exception("Shockwave only supports friend list updates with type 'Update'.");
            if (Updates.Any(update => update.Friend is null))
                throw new Exception($"Null friend when composing {nameof(FriendsUpdatedMsg)} on Shockwave.");
            p.ComposeArray(Updates.Select(x => x.Friend!));
        }
        else
        {
            p.ComposeArray(Categories);
            p.ComposeArray(Updates);
        }
    }
}
