using Xabbo.Messages;

namespace Xabbo.Core;

public class Friend : IFriend, IParserComposer<Friend>
{
    public Id Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public bool IsOnline { get; set; }
    public bool CanFollow { get; set; }
    public string FigureString { get; set; } = string.Empty;
    public Id CategoryId { get; set; }
    public string Motto { get; set; } = string.Empty;
    public string RealName { get; set; } = string.Empty;
    public string FacebookId { get; set; } = string.Empty;
    public bool IsAcceptingOfflineMessages { get; set; }
    public bool IsVipMember { get; set; }
    public bool IsPocketHabboUser { get; set; }
    public Relation Relation { get; set; }

    public Friend() { }

    protected Friend(in PacketReader p)
    {
        UnsupportedClientException.ThrowIf(p.Client, ClientType.Shockwave);

        Id = p.ReadId();
        Name = p.ReadString();
        Gender = H.ToGender(p.ReadInt());
        IsOnline = p.ReadBool();
        CanFollow = p.ReadBool();
        FigureString = p.ReadString();
        CategoryId = p.ReadId();
        Motto = p.ReadString();

        if (p.Client == ClientType.Flash)
        {
            RealName = p.ReadString();
            FacebookId = p.ReadString();
        }

        IsAcceptingOfflineMessages = p.ReadBool();
        IsVipMember = p.ReadBool();
        IsPocketHabboUser = p.ReadBool();

        if (p.Client == ClientType.Unity)
        {
            RealName = p.ReadString();
            FacebookId = p.ReadString();
        }

        Relation = (Relation)p.ReadShort();
    }

    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteId(Id);
        p.WriteString(Name);
        p.WriteInt(Gender.GetValue());
        p.WriteBool(IsOnline);
        p.WriteBool(CanFollow);
        p.WriteString(FigureString);
        p.WriteId(CategoryId);
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
