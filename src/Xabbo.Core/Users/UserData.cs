using System;

using Xabbo.Messages;

namespace Xabbo.Core;

/// <inheritdoc cref="IUserData"/>
public sealed class UserData : IUserData, IParserComposer<UserData>
{
    public Id Id { get; set; }
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
        Id = p.ReadId();
        Name = p.ReadString();
        Figure = p.ReadString();
        Gender = H.ToGender(p.ReadString());
        Motto = p.ReadString();
        RealName = p.ReadString();
        DirectMail = p.ReadBool();
        TotalRespects = p.ReadInt();
        RespectsLeft = p.ReadInt();
        ScratchesLeft = p.ReadInt();
        StreamPublishingAllowed = p.ReadBool();
        LastAccessDate = p.ReadString();
        IsNameChangeable = p.ReadBool();
        IsSafetyLocked = p.ReadBool();

        if (p.Available > 0)
        {
            _Bool5 = p.ReadBool();
        }
    }

    private void ParseOrigins(in PacketReader p)
    {
        string[] lines = p.ReadContent().Split('\r');
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

    void IComposer.Compose(in PacketWriter p)
    {
        if (p.Client is ClientType.Shockwave)
        {
            p.WriteContent(string.Join('\r', [
                $"name={Name}",
                $"figure={Figure}",
                $"sex={Gender.ToClientString().ToLower()}",
                $"customData={CustomData}",
                $"ph_tickets={PoolTickets}",
                $"ph_figure={PoolFigure}",
                $"photo_film={PhotoFilm}",
                $"directMail={(DirectMail ? '1' : '0')}",
                $"onlineStatus={(ShowOnline ? '1' : '0')}",
                $"publicProfileEnabled={(PublicProfileEnabled ? '1' : '0')}",
                $"friendRequestsEnabled={(FriendRequestsEnabled ? '1' : '0')}",
                $"offlineMessagingEnabled={(OfflineMessagingEnabled ? '1' : '0')}",
            ]));
        }
        else
        {
            p.WriteId(Id);
            p.WriteString(Name);
            p.WriteString(Figure);
            p.WriteString(Gender.ToClientString());
            p.WriteString(Motto);
            p.WriteString(RealName);
            p.WriteBool(DirectMail);
            p.WriteInt(TotalRespects);
            p.WriteInt(RespectsLeft);
            p.WriteInt(ScratchesLeft);
            p.WriteBool(StreamPublishingAllowed);
            p.WriteString(LastAccessDate);
            p.WriteBool(IsNameChangeable);
            p.WriteBool(IsSafetyLocked);
            p.WriteBool(_Bool5);
        }
    }

    static UserData IParser<UserData>.Parse(in PacketReader p) => new(in p);
}
