using System;
using System.Collections.Generic;

using Xabbo.Core.Messages;

namespace Xabbo.Core
{
    /// <summary>
    /// The user's information that is sent upon requesting their profile.
    /// </summary>
    public class UserProfile : IUserProfile
    {
        public static UserProfile Parse(IReadOnlyPacket packet) => new UserProfile(packet);

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

        public UserProfile()
        {
            Groups = new List<GroupInfo>();
        }

        protected UserProfile(IReadOnlyPacket packet)
            : this()
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

            int n = packet.ReadInt();
            for (int i = 0; i < n; i++)
                Groups.Add(GroupInfo.Parse(packet));

            LastLogin = TimeSpan.FromSeconds(packet.ReadInt());
            DisplayInClient = packet.ReadBool();
        }

        public void Write(IPacket packet) => packet.WriteValues(
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
    }
}
