using System;
using System.Collections.Generic;

using Xabbo.Common;
using Xabbo.Messages;

namespace Xabbo.Core;

/// <summary>
/// The user's information that is sent upon requesting their profile.
/// </summary>
public class UserProfile : IUserProfile
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
        Groups = new List<GroupInfo>();
    }

    protected UserProfile(IReadOnlyPacket packet)
        : this()
    {
        if (packet.Protocol == ClientType.Flash)
        {
            Id = packet.ReadInt();
            Name = packet.ReadString();
            Figure = packet.ReadString();
            Motto = packet.ReadString();
            Created = packet.ReadString();
            ActivityPoints = packet.ReadInt();
            Friends = packet.ReadInt();
            IsFriend = packet.ReadBool();
            IsFriendRequestSent = packet.ReadBool();
            IsOnline = packet.ReadBool();

            int n = packet.ReadInt();
            for (int i = 0; i < n; i++)
            {
                Groups.Add(GroupInfo.Parse(packet));
            }

            LastLogin = TimeSpan.FromSeconds(packet.ReadInt());
            DisplayInClient = packet.ReadBool();

            if (packet.Available > 0)
            {
                packet.ReadBool();
                Level = packet.ReadInt();
                packet.ReadInt();
                StarGems = packet.ReadInt();
                packet.ReadBool();
                packet.ReadBool();
            }
        }
        else
        {
            Id = packet.ReadLong();
            Name = packet.ReadString();
            Figure = packet.ReadString();
            Motto = packet.ReadString();
            Created = packet.ReadString();
            // ActivityPoints = packet.ReadInt();
            Friends = packet.ReadInt();
            IsFriend = packet.ReadBool();
            // IsFriendRequestSent = packet.ReadBool();
            // IsOnline = packet.ReadBool();

            // long secondsSinceLastLogin
            // bool showInClient ???
            // bool ?
            // int ?
            // int ?
            // int ?
            // bool ?
            // bool ?

            short n = packet.ReadShort();
            for (int i = 0; i < n; i++)
                Groups.Add(GroupInfo.Parse(packet));

            LastLogin = TimeSpan.FromSeconds(packet.ReadInt());
            DisplayInClient = packet.ReadBool();
        }
    }

    public void Compose(IPacket packet) => packet.Write(
        Id,
        Name,
        Figure,
        Motto,
        Created,
        ActivityPoints,
        Friends,
        IsFriend,
        IsFriendRequestSent,
        IsOnline,
        Groups,
        (int)LastLogin.TotalSeconds,
        DisplayInClient
    );


    public static UserProfile Parse(IReadOnlyPacket packet) => new UserProfile(packet);
}
