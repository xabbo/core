using System;
using System.Collections.Generic;
using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class RoomInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int OwnerId { get; set; }
        public string OwnerName { get; set; }
        public RoomAccess Access { get; set; }
        public bool IsOpen => Access == RoomAccess.Open;
        public bool IsDoorbell => Access == RoomAccess.Doorbell;
        public bool IsLocked => Access == RoomAccess.Password;
        public bool IsInvisible => Access == RoomAccess.Invisible;
        public int UserCount { get; set; }
        public int MaxVisitors { get; set; }
        public string Description { get; set; }
        public TradePermissions Trading { get; set; }
        public int Score { get; set; }
        public int Ranking { get; set; }
        public RoomCategory Category { get; set; }
        public List<string> Tags { get; set; }
        
        public RoomFlags Flags { get; set; }
        public bool HasOfficialRoomPic => Flags.HasFlag(RoomFlags.HasOfficialRoomPic);
        public bool IsGroupHomeRoom => Flags.HasFlag(RoomFlags.IsGroupHomeRoom);
        public bool HasEvent => Flags.HasFlag(RoomFlags.HasEvent);
        public bool ShowOwnerName => Flags.HasFlag(RoomFlags.ShowOwnerName);
        public bool AllowPets => Flags.HasFlag(RoomFlags.AllowPets);

        public string OfficialRoomPicRef { get; set; }
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public string GroupBadge { get; set; }
        public string EventName { get; set; }
        public string EventDescription { get; set; }
        public int EventMinutesLeft { get; set; }

        public RoomInfo()
        {
            Tags = new List<string>();
        }

        internal RoomInfo(Packet packet)
            : this()
        {
            Id = packet.ReadInteger();
            Name = packet.ReadString();
            OwnerId = packet.ReadInteger();
            OwnerName = packet.ReadString();
            Access = (RoomAccess)packet.ReadInteger();
            UserCount = packet.ReadInteger();
            MaxVisitors = packet.ReadInteger();
            Description = packet.ReadString();
            Trading = (TradePermissions)packet.ReadInteger();
            Score = packet.ReadInteger();
            Ranking = packet.ReadInteger();
            Category = (RoomCategory)packet.ReadInteger();

            int n = packet.ReadInteger();
            for (int i = 0; i < n; i++)
                Tags.Add(packet.ReadString());

            Flags = (RoomFlags)packet.ReadInteger();

            if (Flags.HasFlag(RoomFlags.HasOfficialRoomPic))
                OfficialRoomPicRef = packet.ReadString();

            if (Flags.HasFlag(RoomFlags.IsGroupHomeRoom))
            {
                GroupId = packet.ReadInteger();
                GroupName = packet.ReadString();
                GroupBadge = packet.ReadString();
            }

            if (Flags.HasFlag(RoomFlags.HasEvent))
            {
                EventName = packet.ReadString();
                EventDescription = packet.ReadString();
                EventMinutesLeft = packet.ReadInteger();
            }
        }

        public static RoomInfo Parse(Packet packet) => new RoomInfo(packet);
    }
}
