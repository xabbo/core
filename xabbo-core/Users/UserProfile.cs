using System;
using System.Collections.Generic;
using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    /// <summary>
    /// The users' information that is sent upon requesting their profile.
    /// </summary>
    public class UserProfile
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Figure { get; set; }
        public string Motto { get; set; }
        public string CreationDate { get; set; }
        public int ActivityPoints { get; set; }
        public int Friends { get; set; }
        public bool IsFriend { get; set; }
        public bool IsFriendRequestSent { get; set; }
        public bool IsOnline { get; set; }
        public List<GroupInfo> Groups { get; set; }
        public TimeSpan LastLogin { get; set; }
        public bool DisplayInClient { get; set; }

        public UserProfile()
        {
            Groups = new List<GroupInfo>();
        }

        public static UserProfile Parse(Packet packet)
        {
            var profile = new UserProfile();

            profile.Id = packet.ReadInteger();
            profile.Name = packet.ReadString();
            profile.Figure = packet.ReadString();
            profile.Motto = packet.ReadString();
            profile.CreationDate = packet.ReadString();
            profile.ActivityPoints = packet.ReadInteger();
            profile.Friends = packet.ReadInteger();
            profile.IsFriend = packet.ReadBoolean();
            profile.IsFriendRequestSent = packet.ReadBoolean();
            profile.IsOnline = packet.ReadBoolean();

            int n = packet.ReadInteger();
            for (int i = 0; i < n; i++)
            {
                profile.Groups.Add(GroupInfo.Parse(packet));
            }

            profile.LastLogin = TimeSpan.FromSeconds(packet.ReadInteger());
            profile.DisplayInClient = packet.ReadBoolean();

            return profile;
        }
    }
}
