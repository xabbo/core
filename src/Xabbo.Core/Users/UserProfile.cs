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
    public string Name { get; set; }
    public string Figure { get; set; }
    public string Motto { get; set; }
    public string Created { get; set; }
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
        Name =
        Figure =
        Motto =
        Created = string.Empty;
        Groups = [];
    }

    private UserProfile(in PacketReader packet) : this()
    {
        if (packet.Client == ClientType.Flash)
        {
            Id = packet.Read<int>();
            Name = packet.Read<string>();
            Figure = packet.Read<string>();
            Motto = packet.Read<string>();
            Created = packet.Read<string>();
            ActivityPoints = packet.Read<int>();
            Friends = packet.Read<int>();
            IsFriend = packet.Read<bool>();
            IsFriendRequestSent = packet.Read<bool>();
            IsOnline = packet.Read<bool>();

            int n = packet.Read<int>();
            for (int i = 0; i < n; i++)
            {
                Groups.Add(GroupInfo.Parse(packet));
            }

            LastLogin = TimeSpan.FromSeconds(packet.Read<int>());
            DisplayInClient = packet.Read<bool>();

            if (packet.Available > 0)
            {
                packet.Read<bool>();
                Level = packet.Read<int>();
                packet.Read<int>();
                StarGems = packet.Read<int>();
                packet.Read<bool>();
                packet.Read<bool>();
            }
        }
        else
        {
            Id = packet.Read<long>();
            Name = packet.Read<string>();
            Figure = packet.Read<string>();
            Motto = packet.Read<string>();
            Created = packet.Read<string>();
            // ActivityPoints = packet.Read<int>();
            Friends = packet.Read<int>();
            IsFriend = packet.Read<bool>();
            // IsFriendRequestSent = packet.Read<bool>();
            // IsOnline = packet.Read<bool>();

            // long secondsSinceLastLogin
            // bool showInClient ???
            // bool ?
            // int ?
            // int ?
            // int ?
            // bool ?
            // bool ?

            short n = packet.Read<short>();
            for (int i = 0; i < n; i++)
                Groups.Add(GroupInfo.Parse(packet));

            LastLogin = TimeSpan.FromSeconds(packet.Read<int>());
            DisplayInClient = packet.Read<bool>();
        }
    }

    public void Compose(in PacketWriter p)
    {
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

    public static UserProfile Parse(in PacketReader packet) => new(in packet);
}
