using System;
using Xabbo.Messages;

namespace Xabbo.Core;

/// <summary>
/// The user's own data that is sent upon requesting user data.
/// </summary>
public class UserData : IUserData
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Figure { get; set; }
    public Gender Gender { get; set; }
    public string Motto { get; set; }
    public string RealName { get; set; }
    public bool DirectMail { get; set; }
    public int TotalRespects { get; set; }
    public int RespectsLeft { get; set; }
    public int ScratchesLeft { get; set; }
    public bool StreamPublishingAllowed { get; set; }
    public string LastAccessDate { get; set; }
    public bool IsNameChangeable { get; set; }
    public bool IsSafetyLocked { get; set; }
    public bool _Bool5 { get; set; }

    public UserData()
    {
        Name =
        Figure =
        Motto =
        RealName =
        LastAccessDate = string.Empty;

        Gender = Gender.Unisex;
    }

    protected UserData(IReadOnlyPacket packet)
    {
        Id = packet.ReadLegacyLong();
        Name = packet.ReadString();
        Figure = packet.ReadString();
        Gender = H.ToGender(packet.ReadString());
        Motto = packet.ReadString();
        RealName = packet.ReadString();
        DirectMail = packet.ReadBool();
        TotalRespects = packet.ReadInt();
        RespectsLeft = packet.ReadInt();
        ScratchesLeft = packet.ReadInt();
        StreamPublishingAllowed = packet.ReadBool();
        LastAccessDate = packet.ReadString();
        IsNameChangeable = packet.ReadBool();
        IsSafetyLocked = packet.ReadBool();

        if (packet.Available > 0)
        {
            _Bool5 = packet.ReadBool();
        }
    }

    public void Compose(IPacket packet) => packet
        .WriteLegacyLong(Id)
        .WriteString(Name)
        .WriteString(Figure)
        .WriteString(Gender.ToShortString())
        .WriteString(Motto)
        .WriteString(RealName)
        .WriteBool(DirectMail)
        .WriteInt(TotalRespects)
        .WriteInt(RespectsLeft)
        .WriteInt(ScratchesLeft)
        .WriteBool(StreamPublishingAllowed)
        .WriteString(LastAccessDate)
        .WriteBool(IsNameChangeable)
        .WriteBool(IsSafetyLocked)
        .WriteBool(_Bool5);

    public static UserData Parse(IReadOnlyPacket packet) => new UserData(packet);
}
