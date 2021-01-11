using System;
using System.Collections.Generic;
using Xabbo.Core.Messages;
using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class RoomInfo : IRoomInfo
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long OwnerId { get; set; }
        public string OwnerName { get; set; }
        public RoomAccess Access { get; set; }
        public bool IsOpen => Access == RoomAccess.Open;
        public bool IsDoorbell => Access == RoomAccess.Doorbell;
        public bool IsLocked => Access == RoomAccess.Password;
        public bool IsInvisible => Access == RoomAccess.Invisible;
        public int Users { get; set; }
        public int MaxUsers { get; set; }
        public string Description { get; set; }
        public TradePermissions Trading { get; set; }
        public int Score { get; set; }
        public int Ranking { get; set; }
        public RoomCategory Category { get; set; }
        public List<string> Tags { get; set; }
        IReadOnlyList<string> IRoomInfo.Tags => Tags;
        
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
            Name =
            OwnerName =
            Description =
            OfficialRoomPicRef =
            GroupName =
            GroupBadge =
            EventName = 
            EventDescription = string.Empty;
            Tags = new List<string>();
        }

        protected RoomInfo(IReadOnlyPacket packet)
            : this()
        {
            Id = packet.ReadLong();
            Name = packet.ReadString();
            OwnerId = packet.ReadLong();
            OwnerName = packet.ReadString();
            Access = (RoomAccess)packet.ReadInt();
            Users = packet.ReadInt();
            MaxUsers = packet.ReadInt();
            Description = packet.ReadString();
            Trading = (TradePermissions)packet.ReadInt();
            Score = packet.ReadInt();
            Ranking = packet.ReadInt();
            Category = (RoomCategory)packet.ReadInt();

            int n = packet.ReadShort();
            for (int i = 0; i < n; i++)
                Tags.Add(packet.ReadString());

            Flags = (RoomFlags)packet.ReadInt();

            if (Flags.HasFlag(RoomFlags.HasOfficialRoomPic))
                OfficialRoomPicRef = packet.ReadString();

            if (Flags.HasFlag(RoomFlags.IsGroupHomeRoom))
            {
                GroupId = packet.ReadInt();
                GroupName = packet.ReadString();
                GroupBadge = packet.ReadString();
            }

            if (Flags.HasFlag(RoomFlags.HasEvent))
            {
                EventName = packet.ReadString();
                EventDescription = packet.ReadString();
                EventMinutesLeft = packet.ReadInt();
            }
        }

        public static RoomInfo Parse(IReadOnlyPacket packet) => new RoomInfo(packet);

        public virtual void Write(IPacket packet)
        {
            packet.WriteLong(Id);
            packet.WriteString(Name);

            packet.WriteLong(OwnerId);
            packet.WriteString(OwnerName);
            packet.WriteInt((int)Access);
            packet.WriteInt(Users);
            packet.WriteInt(MaxUsers);
            packet.WriteString(Description);
            packet.WriteInt((int)Trading);
            packet.WriteInt(Score);
            packet.WriteInt(Ranking);
            packet.WriteInt((int)Category);

            packet.WriteShort((short)Tags.Count);
            foreach (string tag in Tags)
                packet.WriteString(tag);

            packet.WriteInt((int)Flags);

            if (Flags.HasFlag(RoomFlags.HasOfficialRoomPic))
                packet.WriteString(OfficialRoomPicRef);

            if (Flags.HasFlag(RoomFlags.IsGroupHomeRoom))
            {
                packet.WriteInt(GroupId);
                packet.WriteString(GroupName);
                packet.WriteString(GroupBadge);
            }

            if (Flags.HasFlag(RoomFlags.HasEvent))
            {
                packet.WriteString(EventName);
                packet.WriteString(EventDescription);
                packet.WriteInt(EventMinutesLeft);
            }
        }
    }
}
