using Xabbo.Messages;

namespace Xabbo.Core;

/// <summary>
/// Represents a user search result.
/// </summary>
public sealed class UserSearchResult : IParserComposer<UserSearchResult>
{
    public Id Id { get; set; }
    public string Name { get; set; } = "";
    public string Motto { get; set; } = "";
    public bool Online { get; set; }
    public bool CanFollow { get; set; }
    public string LastAccess { get; set; } = "";
    public Gender Gender { get; set; } = Gender.None;
    public string Figure { get; set; } = "";
    public string RealName { get; set; } = "";

    /// <summary>
    /// The current location of the user.
    /// </summary>
    /// <remarks>
    /// Only available on Shockwave.
    /// </remarks>
    public string Location { get; set; } = "";

    public UserSearchResult() { }

    private UserSearchResult(in PacketReader p)
    {
        if (p.Client is ClientType.Shockwave)
        {
            Id = p.ReadId();
            Name = p.ReadString();
            Gender = H.ToGender(p.ReadInt());
            Motto = p.ReadString();
            Online = p.ReadBool();
            Location = p.ReadString();
            LastAccess = p.ReadString();
            Figure = p.ReadString();
        }
        else
        {
            Id = p.ReadId();
            Name = p.ReadString();
            Motto = p.ReadString();
            Online = p.ReadBool();
            CanFollow = p.ReadBool();
            LastAccess = p.ReadString();
            Gender = H.ToGender(p.ReadInt());
            Figure = p.ReadString();
            RealName = p.ReadString();
        }
    }

    void IComposer.Compose(in PacketWriter p)
    {
        if (p.Client is ClientType.Shockwave)
        {
            p.WriteId(Id);
            p.WriteString(Name);
            p.WriteInt((int)Gender);
            p.WriteString(Motto);
            p.WriteBool(Online);
            p.WriteString(Location);
            p.WriteString(LastAccess);
            p.WriteString(Figure);
        }
        else
        {
            p.WriteId(Id);
            p.WriteString(Name);
            p.WriteString(Motto);
            p.WriteBool(Online);
            p.WriteBool(CanFollow);
            p.WriteString(LastAccess);
            p.WriteInt((int)Gender);
            p.WriteString(Figure);
            p.WriteString(RealName);
        }
    }

    static UserSearchResult IParser<UserSearchResult>.Parse(in PacketReader p) => new(in p);
}
