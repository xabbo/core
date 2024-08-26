using System;
using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core;

/// <summary>
/// The user's information that is sent upon requesting their profile.
/// </summary>
public sealed class UserProfile : IUserProfile, IComposer, IParser<UserProfile>
{
    public long Id { get; set; }
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

        Id = p.Read<int>();
        Name = p.Read<string>();
        Figure = p.Read<string>();
        Motto = p.Read<string>();
        Created = p.Read<string>();
        ActivityPoints = p.Read<int>();
        Friends = p.Read<int>();
        IsFriend = p.Read<bool>();
        IsFriendRequestSent = p.Read<bool>();
        IsOnline = p.Read<bool>();

        int n = p.Read<int>();
        for (int i = 0; i < n; i++)
        {
            Groups.Add(GroupInfo.Parse(p));
        }

        LastLogin = TimeSpan.FromSeconds(p.Read<int>());
        DisplayInClient = p.Read<bool>();

        if (p.Available > 0)
        {
            p.Read<bool>();
            Level = p.Read<int>();
            p.Read<int>();
            StarGems = p.Read<int>();
            p.Read<bool>();
            p.Read<bool>();
        }
    }

    public void Compose(in PacketWriter p)
    {
        UnsupportedClientException.ThrowIf(p.Client, ClientType.Shockwave);

        p.Write(Id);
        p.Write(Name);
        p.Write(Figure);
        p.Write(Motto);
        p.Write(Created);
        p.Write(ActivityPoints);
        p.Write(Friends);
        p.Write(IsFriend);
        p.Write(IsFriendRequestSent);
        p.Write(IsOnline);
        p.Write(Groups);
        p.Write((int)LastLogin.TotalSeconds);
        p.Write(DisplayInClient);
    }

    public static UserProfile Parse(in PacketReader p) => new(in p);
}
