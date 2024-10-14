using System;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

/// <summary>
/// Sent when banning a user from a room.
/// <para/>
/// Supported clients: <see cref="ClientType.All"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="Out.BanUserWithDuration"/></item>
/// <item>Shockwave: <see cref="Xabbo.Messages.Shockwave.Out.ROOMBAN"/></item>
/// </list>
/// </summary>
/// <param name="Id">The ID of the user to ban. Applies to <see cref="ClientType.Modern"/> clients.</param>
/// <param name="Name">The name of the user to ban. Applies to the <see cref="ClientType.Origins"/> client.</param>
/// <param name="RoomId">The ID of the room to ban the user from. Applies to <see cref="ClientType.Modern"/> clients.</param>
/// <param name="Duration">The duration of the ban.</param>
/// <param name="DurationString">A custom ban duration string. Used when <see cref="Duration"/> is -1.</param>
public sealed record BanUserMsg(
    Id? Id,
    string? Name,
    Id? RoomId,
    BanDuration Duration,
    string? DurationString = null
)
    : IMessage<BanUserMsg>
{
    /// <summary>
    /// Constructs a new <see cref="BanUserMsg"/> with the specified user, room ID and duration.
    /// </summary>
    /// <param name="user">The user to ban.</param>
    /// <param name="roomId"><inheritdoc cref="BanUserMsg" path="/param[@name='RoomId']"/></param>
    /// <param name="duration"><inheritdoc cref="BanUserMsg" path="/param[@name='Duration']"/></param>
    public BanUserMsg(IUser user, Id roomId, BanDuration duration = BanDuration.Permanent)
        : this(user.Id, user.Name, roomId, duration) { }

    /// <summary>
    /// Constructs a new <see cref="BanUserMsg"/> with the specified user/ID pair, room ID and duration.
    /// </summary>
    /// <param name="user">The ID and name of the user to ban.</param>
    /// <param name="roomId"><inheritdoc cref="BanUserMsg" path="/param[@name='RoomId']"/></param>
    /// <param name="duration"><inheritdoc cref="BanUserMsg" path="/param[@name='Duration']"/></param>
    public BanUserMsg(IdName user, Id roomId, BanDuration duration = BanDuration.Permanent)
        : this(user.Id, user.Name, roomId, duration) { }

    static Identifier IMessage<BanUserMsg>.Identifier => Out.BanUserWithDuration;

    static BanUserMsg IParser<BanUserMsg>.Parse(in PacketReader p)
    {
        Id? id = null;
        Id? roomId = null;
        string? name = null;
        BanDuration duration;

        if (p.Client is ClientType.Shockwave)
        {
            name = p.ReadString();
        }
        else
        {
            id = p.ReadId();
            roomId = p.ReadId();
        }

        string durationString = p.ReadString();

        duration = durationString switch
        {
            string s when s.Equals(BanDuration.Hour.ToClientString(p.Client)) => BanDuration.Day,
            string s when s.Equals(BanDuration.Day.ToClientString(p.Client)) => BanDuration.Day,
            string s when s.Equals(BanDuration.Permanent.ToClientString(p.Client)) => BanDuration.Permanent,
            _ => (BanDuration)(-1)
        };

        return new BanUserMsg(id, name, roomId, duration, duration == (BanDuration)(-1) ? durationString : null);
    }

    void IComposer.Compose(in PacketWriter p)
    {
        if (p.Client is ClientType.Shockwave)
        {
            if (Name is null)
                throw new Exception($"{nameof(Name)} is required when composing {nameof(BanUserMsg)} on {p.Client}.");
            p.WriteString(Name);
        }
        else
        {
            if (Id is not { } id)
                throw new Exception($"{nameof(Id)} is required when composing {nameof(BanUserMsg)} on {p.Client}.");
            if (RoomId is not { } roomId)
                throw new Exception($"{nameof(RoomId)} is required when composing {nameof(BanUserMsg)} on {p.Client}.");
            p.WriteId(id);
            p.WriteId(roomId);
        }

        if (Duration == (BanDuration)(-1))
            p.WriteString(DurationString ?? "");
        else
            p.WriteString(Duration.ToClientString(p.Client));
    }
}