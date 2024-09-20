using Xabbo.Messages;

namespace Xabbo.Core;

public class Friend : IFriend, IParserComposer<Friend>
{
    public Id Id { get; set; }
    public string Name { get; set; } = "";
    public Gender Gender { get; set; }
    public bool IsOnline { get; set; }
    public bool CanFollow { get; set; }
    public string Figure { get; set; } = "";
    public int CategoryId { get; set; }
    public string Motto { get; set; } = "";
    public string RealName { get; set; } = "";
    public string FacebookId { get; set; } = "";
    public bool IsAcceptingOfflineMessages { get; set; }
    public bool IsVipMember { get; set; }
    public bool IsPocketHabboUser { get; set; }
    public Relation Relation { get; set; }

    // Shockwave fields
    public string Location { get; set; } = "";
    public string LastAccess { get; set; } = "";

    public Friend() { }

    protected Friend(in PacketReader p)
    {
        if (p.Client is ClientType.Shockwave)
        {
            Id = p.ReadId();
            Name = p.ReadString();
            Gender = H.ToGender(p.ReadInt());
            Motto = p.ReadString();
            IsOnline = p.ReadBool();
            Location = p.ReadString();
            LastAccess = p.ReadString();
            Figure = p.ReadString();
            return;
        }

        Id = p.ReadId();
        Name = p.ReadString();
        Gender = H.ToGender(p.ReadInt());
        IsOnline = p.ReadBool();
        CanFollow = p.ReadBool();
        Figure = p.ReadString();
        CategoryId = p.ReadInt();
        Motto = p.ReadString();

        if (p.Client is ClientType.Flash)
        {
            RealName = p.ReadString();
            FacebookId = p.ReadString();
        }

        IsAcceptingOfflineMessages = p.ReadBool();
        IsVipMember = p.ReadBool();
        IsPocketHabboUser = p.ReadBool();

        if (p.Client is ClientType.Unity)
        {
            RealName = p.ReadString();
            FacebookId = p.ReadString();
        }

        Relation = (Relation)p.ReadShort();
    }

    void IComposer.Compose(in PacketWriter p)
    {
        if (p.Client is ClientType.Shockwave)
        {
            p.WriteId(Id);
            p.WriteString(Name);
            p.WriteInt((int)Gender);
            p.WriteString(Motto);
            p.WriteBool(IsOnline);
            p.WriteString(Location);
            p.WriteString(LastAccess);
            p.WriteString(Figure);
            return;
        }

        p.WriteId(Id);
        p.WriteString(Name);
        p.WriteInt(Gender.ToClientValue());
        p.WriteBool(IsOnline);
        p.WriteBool(CanFollow);
        p.WriteString(Figure);
        p.WriteInt(CategoryId);
        p.WriteString(Motto);
        p.WriteString(RealName);
        p.WriteString(FacebookId);
        p.WriteBool(IsAcceptingOfflineMessages);
        p.WriteBool(IsVipMember);
        p.WriteBool(IsPocketHabboUser);
        p.WriteShort((short)Relation);
    }

    public override string ToString() => Name;

    static Friend IParser<Friend>.Parse(in PacketReader p) => new(in p);
}
