using System;
using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core;

/// <inheritdoc cref="IUserProfile"/>
public sealed class UserProfile : IUserProfile, IParserComposer<UserProfile>
{
    public Id Id { get; set; }
    public string Name { get; set; } = "";
    public string Figure { get; set; } = "";
    public string Motto { get; set; } = "";
    public string Created { get; set; } = "";
    public int ActivityPoints { get; set; }
    public int Friends { get; set; }
    public bool IsFriend { get; set; }
    public bool IsFriendRequestSent { get; set; }
    public bool IsOnline { get; set; }
    public List<GroupInfo> Groups { get; set; }
    IReadOnlyList<IGroupInfo> IUserProfile.Groups => Groups;
    public TimeSpan LastLogin { get; set; }
    public bool DisplayInClient { get; set; }
    public int Level { get; set; }
    public int StarGems { get; set; }

    public UserProfile()
    {
        Groups = [];
    }

    private UserProfile(in PacketReader p) : this()
    {
        UnsupportedClientException.ThrowIf(p.Client, ClientType.Shockwave);

        Id = p.ReadInt();
        Name = p.ReadString();
        Figure = p.ReadString();
        Motto = p.ReadString();
        Created = p.ReadString();
        ActivityPoints = p.ReadInt();
        Friends = p.ReadInt();
        IsFriend = p.ReadBool();
        IsFriendRequestSent = p.ReadBool();
        IsOnline = p.ReadBool();
        Groups = [.. p.ParseArray<GroupInfo>()];
        LastLogin = TimeSpan.FromSeconds(p.ReadInt());
        DisplayInClient = p.ReadBool();

        if (p.Available > 0)
        {
            p.ReadBool();
            Level = p.ReadInt();
            p.ReadInt();
            StarGems = p.ReadInt();
            p.ReadBool();
            p.ReadBool();
        }
    }

    void IComposer.Compose(in PacketWriter p)
    {
        UnsupportedClientException.ThrowIf(p.Client, ClientType.Shockwave);

        p.WriteId(Id);
        p.WriteString(Name);
        p.WriteString(Figure);
        p.WriteString(Motto);
        p.WriteString(Created);
        p.WriteInt(ActivityPoints);
        p.WriteInt(Friends);
        p.WriteBool(IsFriend);
        p.WriteBool(IsFriendRequestSent);
        p.WriteBool(IsOnline);
        p.ComposeArray(Groups);
        p.WriteInt((int)LastLogin.TotalSeconds);
        p.WriteBool(DisplayInClient);
    }

    static UserProfile IParser<UserProfile>.Parse(in PacketReader p) => new(in p);
}
