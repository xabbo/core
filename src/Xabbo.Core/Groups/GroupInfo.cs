using Xabbo.Messages;

namespace Xabbo.Core;

/// <inheritdoc cref="IGroupInfo"/>
public sealed class GroupInfo : IGroupInfo, IParserComposer<GroupInfo>
{
    public Id Id { get; set; }
    public string Name { get; set; } = "";
    public string BadgeCode { get; set; } = "";
    public string PrimaryColor { get; set; } = "";
    public string SecondaryColor { get; set; } = "";
    public bool IsFavorite { get; set; }
    public Id OwnerId { get; set; }
    public bool HasForum { get; set; }

    public GroupInfo() { }

    private GroupInfo(in PacketReader p)
    {
        UnsupportedClientException.ThrowIf(p.Client, ClientType.Shockwave);

        Id = p.ReadId();
        Name = p.ReadString();
        BadgeCode = p.ReadString();
        PrimaryColor = p.ReadString();
        SecondaryColor = p.ReadString();
        IsFavorite = p.ReadBool();
        OwnerId = p.ReadId();
        HasForum = p.ReadBool();
    }

    void IComposer.Compose(in PacketWriter p)
    {
        UnsupportedClientException.ThrowIf(p.Client, ClientType.Shockwave);

        p.WriteId(Id);
        p.WriteString(Name);
        p.WriteString(BadgeCode);
        p.WriteString(PrimaryColor);
        p.WriteString(SecondaryColor);
        p.WriteBool(IsFavorite);
        p.WriteId(OwnerId);
        p.WriteBool(HasForum);
    }

    static GroupInfo IParser<GroupInfo>.Parse(in PacketReader p) => new(in p);
}
