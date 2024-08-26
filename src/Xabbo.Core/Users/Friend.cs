using System;

using Xabbo.Messages;

namespace Xabbo.Core;

public class Friend : IFriend, IComposer, IParser<Friend>
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
        Id = p.Read<Id>();
        Name = p.Read<string>();
        Gender = H.ToGender(p.Read<int>());
        IsOnline = p.Read<bool>();
        CanFollow = p.Read<bool>();
        FigureString = p.Read<string>();
        CategoryId = p.Read<Id>();
        Motto = p.Read<string>();

        if (p.Client == ClientType.Flash)
        {
            RealName = p.Read<string>();
            FacebookId = p.Read<string>();
        }

        IsAcceptingOfflineMessages = p.Read<bool>();
        IsVipMember = p.Read<bool>();
        IsPocketHabboUser = p.Read<bool>();

        if (p.Client == ClientType.Unity)
        {
            RealName = p.Read<string>();
            FacebookId = p.Read<string>();
        }

        Relation = (Relation)p.Read<short>();
    }

    public void Compose(in PacketWriter p)
    {
        p.Write(Id);
        p.Write(Name);
        p.Write(Gender.GetValue());
        p.Write(IsOnline);
        p.Write(CanFollow);
        p.Write(FigureString);
        p.Write(CategoryId);
        p.Write(Motto);
        p.Write(RealName);
        p.Write(FacebookId);
        p.Write(IsAcceptingOfflineMessages);
        p.Write(IsVipMember);
        p.Write(IsPocketHabboUser);
        p.Write((short)Relation);
    }

    public override string ToString() => Name;

    public static Friend Parse(in PacketReader p) => new(in p);
}
