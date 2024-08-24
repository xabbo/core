using Xabbo.Messages;

namespace Xabbo.Core;

/// <summary>
/// The user's own data that is sent upon requesting user data.
/// </summary>
public sealed class UserData : IUserData, IComposer, IParser<UserData>
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

    private UserData(in PacketReader packet)
    {
        Id = packet.Read<Id>();
        Name = packet.Read<string>();
        Figure = packet.Read<string>();
        Gender = H.ToGender(packet.Read<string>());
        Motto = packet.Read<string>();
        RealName = packet.Read<string>();
        DirectMail = packet.Read<bool>();
        TotalRespects = packet.Read<int>();
        RespectsLeft = packet.Read<int>();
        ScratchesLeft = packet.Read<int>();
        StreamPublishingAllowed = packet.Read<bool>();
        LastAccessDate = packet.Read<string>();
        IsNameChangeable = packet.Read<bool>();
        IsSafetyLocked = packet.Read<bool>();

        if (packet.Available > 0)
        {
            _Bool5 = packet.Read<bool>();
        }
    }

    public void Compose(in PacketWriter p)
    {
        p.Write(Id);
        p.Write(Name);
        p.Write(Figure);
        p.Write(Gender.ToShortString());
        p.Write(Motto);
        p.Write(RealName);
        p.Write(DirectMail);
        p.Write(TotalRespects);
        p.Write(RespectsLeft);
        p.Write(ScratchesLeft);
        p.Write(StreamPublishingAllowed);
        p.Write(LastAccessDate);
        p.Write(IsNameChangeable);
        p.Write(IsSafetyLocked);
        p.Write(_Bool5);
    }

    public static UserData Parse(in PacketReader packet) => new(in packet);
}
