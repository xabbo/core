using System;

using Xabbo.Messages;

namespace Xabbo.Core;

public class Friend : IFriend, IComposer, IParser<Friend>
{
    public static Friend Parse(in PacketReader packet) => new(packet);

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

    protected Friend(in PacketReader packet)
    {
        Id = packet.Read<Id>();
        Name = packet.Read<string>();
        Gender = H.ToGender(packet.Read<int>());
        IsOnline = packet.Read<bool>();
        CanFollow = packet.Read<bool>();
        FigureString = packet.Read<string>();
        CategoryId = packet.Read<Id>();
        Motto = packet.Read<string>();

        if (packet.Client == ClientType.Flash)
        {
            RealName = packet.Read<string>();
            FacebookId = packet.Read<string>();
        }

        IsAcceptingOfflineMessages = packet.Read<bool>();
        IsVipMember = packet.Read<bool>();
        IsPocketHabboUser = packet.Read<bool>();

        if (packet.Client == ClientType.Unity)
        {
            RealName = packet.Read<string>();
            FacebookId = packet.Read<string>();
        }

        Relation = (Relation)packet.Read<short>();
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
}
