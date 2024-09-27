using System.Collections.Generic;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming.Modern;

/// <summary>
/// Received when the user's friend list is updated.
/// <para/>
/// Supported clients: <see cref="ClientType.Modern"/>.
/// </summary>
public sealed class FriendListUpdateMsg : IMessage<FriendListUpdateMsg>
{
    static ClientType IMessage<FriendListUpdateMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<FriendListUpdateMsg>.Identifier => In.FriendListUpdate;

    public List<FriendCategory> Categories { get; set; } = [];
    public List<FriendUpdate> Updates { get; set; } = [];

    static FriendListUpdateMsg IParser<FriendListUpdateMsg>.Parse(in PacketReader p)
    {
        return new()
        {
            Categories = [.. p.ParseArray<FriendCategory>()],
            Updates = [.. p.ParseArray<FriendUpdate>()],
        };
    }

    void IComposer.Compose(in PacketWriter p)
    {
        p.ComposeArray(Categories);
        p.ComposeArray(Updates);
    }
}
