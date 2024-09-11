using System;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

public sealed record BanUserMsg(Id? Id = null, Id? RoomId = null, string? Name = null, BanDuration Duration = BanDuration.Permanent) : IMessage<BanUserMsg>
{
    public BanUserMsg(IUser user, Id roomId, BanDuration duration = BanDuration.Permanent)
        : this(user.Id, roomId, user.Name, duration) { }

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

        duration = p.ReadString() switch
        {
            string s when s.Equals(BanDuration.Hour.ToClientString(p.Client)) => BanDuration.Day,
            string s when s.Equals(BanDuration.Day.ToClientString(p.Client)) => BanDuration.Day,
            string s when s.Equals(BanDuration.Permanent.ToClientString(p.Client)) => BanDuration.Permanent,
            string s => throw new Exception($"Unknown {p.Client} ban duration: '{s}'.")
        };

        return new BanUserMsg
        {
            Id = id,
            RoomId = roomId,
            Name = name,
            Duration = duration
        };
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

        p.WriteString(Duration.ToClientString(p.Client));
    }
}