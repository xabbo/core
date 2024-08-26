using System;

using Xabbo.Messages;

namespace Xabbo.Core;

/// <summary>
/// The user's own data that is sent upon requesting user data.
/// </summary>
public sealed class UserData : IUserData, IComposer, IParser<UserData>
{
    public long Id { get; set; }
    public string Name { get; set; } = "";
    public string Figure { get; set; } = "";
    public Gender Gender { get; set; }
    public string Motto { get; set; } = "";
    public string RealName { get; set; } = "";
    public bool DirectMail { get; set; }
    public int TotalRespects { get; set; }
    public int RespectsLeft { get; set; }
    public int ScratchesLeft { get; set; }
    public bool StreamPublishingAllowed { get; set; }
    public string LastAccessDate { get; set; } = "";
    public bool IsNameChangeable { get; set; }
    public bool IsSafetyLocked { get; set; }
    public bool _Bool5 { get; set; }

    public string CustomData { get; set; } = "";
    public int PhotoFilm { get; set; }
    public int PoolTickets { get; set; }
    public string PoolFigure { get; set; } = "";
    public bool ShowOnline { get; set; }
    public bool PublicProfileEnabled { get; set; }
    public bool FriendRequestsEnabled { get; set; }
    public bool OfflineMessagingEnabled { get; set; }

    public UserData() { }

    private UserData(in PacketReader p)
    {
        switch (p.Client)
        {
            case ClientType.Unity or ClientType.Flash:
                ParseModern(in p);
                break;
            case ClientType.Shockwave:
                ParseOrigins(in p);
                break;
            default:
                throw new UnsupportedClientException(p.Client);
        }
    }

    private void ParseModern(in PacketReader p)
    {
        Id = p.Read<Id>();
        Name = p.Read<string>();
        Figure = p.Read<string>();
        Gender = H.ToGender(p.Read<string>());
        Motto = p.Read<string>();
        RealName = p.Read<string>();
        DirectMail = p.Read<bool>();
        TotalRespects = p.Read<int>();
        RespectsLeft = p.Read<int>();
        ScratchesLeft = p.Read<int>();
        StreamPublishingAllowed = p.Read<bool>();
        LastAccessDate = p.Read<string>();
        IsNameChangeable = p.Read<bool>();
        IsSafetyLocked = p.Read<bool>();

        if (p.Available > 0)
        {
            _Bool5 = p.Read<bool>();
        }
    }

    private void ParseOrigins(in PacketReader p)
    {
        string[] lines = p.Read<string>().Split('\r');
        foreach (string line in lines)
        {
            string[] fields = line.Split('=', 2);
            if (fields.Length != 2)
                throw new Exception($"Invalid entry in UserData: {line}");

            switch (fields[0])
            {
                case "name":
                    Name = fields[1];
                    break;
                case "figure":
                    Figure = fields[1];
                    break;
                case "sex":
                    Gender = H.ToGender(fields[1]);
                    break;
                case "customData":
                    CustomData = fields[1];
                    break;
                case "ph_tickets":
                    PoolTickets = int.Parse(fields[1]);
                    break;
                case "ph_figure":
                    PoolFigure = fields[1];
                    break;
                case "photo_film":
                    PhotoFilm = int.Parse(fields[1]);
                    break;
                case "directMail":
                    DirectMail = fields[1] != "0";
                    break;
                case "onlineStatus":
                    ShowOnline = fields[1] != "0";
                    break;
                case "publicProfileEnabled":
                    PublicProfileEnabled = fields[1] != "0";
                    break;
                case "friendRequestsEnabled":
                    FriendRequestsEnabled = fields[1] != "0";
                    break;
                case "offlineMessagingEnabled":
                    OfflineMessagingEnabled = fields[1] != "0";
                    break;
            }
        }
    }

    public void Compose(in PacketWriter p)
    {
        UnsupportedClientException.ThrowIf(p.Client, ClientType.Shockwave);

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

    public static UserData Parse(in PacketReader p) => new(in p);
}
