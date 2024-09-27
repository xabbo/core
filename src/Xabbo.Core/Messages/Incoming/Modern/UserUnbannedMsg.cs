using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming.Modern;

/// <summary>
/// Received when a user is unbanned from a room.
/// <para/>
/// Response for <see cref="Outgoing.Modern.UnbanUserMsg"/>.
/// <para/>
/// Supported clients: <see cref="ClientType.Modern"/>.
/// </summary>
/// <param name="RoomId">The ID of the room that the user was unbanned from.</param>
/// <param name="UserId">The ID of the user that was unbanned.</param>
public sealed record UserUnbannedMsg(Id RoomId, Id UserId) : IMessage<UserUnbannedMsg>
{
    static ClientType IMessage<UserUnbannedMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<UserUnbannedMsg>.Identifier => In.UserUnbannedFromRoom;
    static UserUnbannedMsg IParser<UserUnbannedMsg>.Parse(in PacketReader p) => new(
        RoomId: p.ReadId(),
        UserId: p.ReadId()
    );
    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteId(RoomId);
        p.WriteId(UserId);
    }
}
