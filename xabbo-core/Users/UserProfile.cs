using System;
using System.Collections.Generic;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    /// <summary>
    /// The user's information that is sent upon requesting their profile.
    /// </summary>
    public class UserProfile : IUserProfile
    {
        public int Id { get; set; }
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

        public static UserProfile Parse(Packet packet)
        {
            var profile = new UserProfile();

            profile.Id = packet.ReadInt();
            profile.Name = packet.ReadString();
            profile.Figure = packet.ReadString();
            profile.Motto = packet.ReadString();
            profile.Created = packet.ReadString();
            profile.ActivityPoints = packet.ReadInt();
            profile.Friends = packet.ReadInt();
            profile.IsFriend = packet.ReadBool();
            profile.IsFriendRequestSent = packet.ReadBool();
            profile.IsOnline = packet.ReadBool();

            int n = packet.ReadInt();
            for (int i = 0; i < n; i++)
            {
                profile.Groups.Add(GroupInfo.Parse(packet));
            }

            profile.LastLogin = TimeSpan.FromSeconds(packet.ReadInt());
            profile.DisplayInClient = packet.ReadBool();

            return profile;
        }
    }
}
