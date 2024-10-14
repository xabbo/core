using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Received when a friend request is received.
/// <para/>
/// Supported clients: <see cref="ClientType.All"/>.
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="In.NewFriendRequest"/></item>
/// <item>Shockwave: <see cref="Xabbo.Messages.Shockwave.In.FRIEND_REQUEST"/></item>
/// </list>
/// </summary>
/// <param name="Id">The ID of the user who sent the request.</param>
/// <param name="Name">The name of the user who sent the request.</param>
/// <param name="FigureString">The figure of the user who sent the request.</param>
public sealed record FriendRequestMsg(Id Id, string Name, string? FigureString) : IMessage<FriendRequestMsg>
{
    static Identifier IMessage<FriendRequestMsg>.Identifier => In.NewFriendRequest;

    static FriendRequestMsg IParser<FriendRequestMsg>.Parse(in PacketReader p) => new(
        Id: p.ReadId(),
        Name: p.ReadString(),
        FigureString: p.Client is not ClientType.Shockwave ? p.ReadString() : null
    );

    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteId(Id);
        p.WriteString(Name);
        if (p.Client is not ClientType.Shockwave)
            p.WriteString(FigureString ?? "");
    }
}
