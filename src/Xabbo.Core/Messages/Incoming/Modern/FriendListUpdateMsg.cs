using System.Collections.Generic;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming.Modern;

public sealed class FriendListUpdateMsg : IMessage<FriendListUpdateMsg>
{
    static bool IMessage<FriendListUpdateMsg>.UseTargetedIdentifiers => true;
    static Identifier IMessage<FriendListUpdateMsg>.Identifier => In.FriendListUpdate;

    public List<FriendCategory> Categories { get; set; } = [];
    public List<FriendUpdate> Updates { get; set; } = [];

    static FriendListUpdateMsg IParser<FriendListUpdateMsg>.Parse(in PacketReader p)
    {
        UnsupportedClientException.ThrowIfNoneOr(p.Client, ClientType.Shockwave);

        return new()
        {
            Categories = [.. p.ParseArray<FriendCategory>()],
            Updates = [.. p.ParseArray<FriendUpdate>()],
        };
    }

    void IComposer.Compose(in PacketWriter p)
    {
        UnsupportedClientException.ThrowIfNoneOr(p.Client, ClientType.Shockwave);

        p.ComposeArray(Categories);
        p.ComposeArray(Updates);
    }
}