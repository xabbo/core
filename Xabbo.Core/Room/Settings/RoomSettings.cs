using System;
using System.Collections.Generic;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class RoomSettings : IPacketData
    {
        public static RoomSettings Parse(IReadOnlyPacket packet) => new RoomSettings(packet);

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public RoomAccess Access { get; set; }
        public string Password { get; set; }
        public int MaxVisitors { get; set; }
        public int UnknownIntA { get; set; }
        public RoomCategory Category { get; set; }
        public List<string> Tags { get; set; }
        public TradePermissions Trading { get; set; }

        public bool AllowPets { get; set; }
        public bool AllowOthersPetsToEat { get; set; }
        public bool DisableRoomBlocking { get; set; }
        public bool HideWalls { get; set; }
        public Thickness WallThickness { get; set; }
        public Thickness FloorThickness { get; set; }

        public ModerationSettings Moderation { get; set; }
        public ChatSettings Chat { get; set; }

        public bool EnlistByFurniContent { get; set; }

        public RoomSettings()
        {
            Name = string.Empty;
            Description = string.Empty;
            Password = string.Empty;

            Tags = new List<string>();
        }

        protected RoomSettings(IReadOnlyPacket packet)
            : this()
        {
            /*
            << RoomSettings
            long roomId
            string name
            string desc
            int access
            int category
            int allowPets

            UNKNOWN DATA

            ---- LEGACY ----
            int roomId
            string name
            string desc
            int access
            int category
            int maxUsers
            int 50 ???
            int tags[] { string }
            int trading
            int allowPets
            int allowOtherPetsToEat
            int disableRoomBlocking
            int hideWalls
            int wallThickness
            int floorThickness
            chatSettings
            bool enlistByFurniContent
            moderationSettings
            */

            Id = packet.ReadLong();
            Name = packet.ReadString();
            Description = packet.ReadString();
            Access = (RoomAccess)packet.ReadInt();
            Category = (RoomCategory)packet.ReadInt();
            AllowPets = packet.ReadInt() > 0;

            // 18 unknown (null) bytes

            //MaxVisitors = packet.ReadInt();
            //UnknownIntA = packet.ReadInt();
            //short n = packet.ReadShort();
            //for (int i = 0; i < n; i++)
            //    Tags.Add(packet.ReadString());
            //Trading = (TradePermissions)packet.ReadInt();
            //AllowPets = packet.ReadInt() > 0;
            //AllowOthersPetsToEat = packet.ReadInt() > 0;
            //DisableRoomBlocking = packet.ReadInt() > 0;
            //HideWalls = packet.ReadInt() > 0;
            //WallThickness = (Thickness)packet.ReadInt();
            //FloorThickness = (Thickness)packet.ReadInt();
            //Chat = ChatSettings.Parse(packet);
            //EnlistByFurniContent = packet.ReadBool();
            //Moderation = ModerationSettings.Parse(packet);
        }

        /*
        ----- LEGACY -----
        >> RoomSettingsSave
        int id
        string name
        string description
        int access
        string password
        int maxUsers
        int category
        int tags { string }
        int tradePermissions
        bool allowPets
        bool allowOthersPetsToEat
        bool disableRoomBlocking
        bool hideWalls
        int wallThickness
        int floorThickness
        moderationSettings
          int 
          int 
          int 
        chatSettings
          int 
          int 
          int 
          int chatHearingDistance
          int 
        bool enlistByTopFurniContent
        */

        /// <summary>
        /// Writes the values of this <see cref="RoomSettings"/> to the specified packet
        /// to be sent to the server with <c>RoomSettingsSave</c>.
        /// </summary>
        public void Write(IPacket packet)
        {
            packet.WriteValues(
                Id,
                Name ?? string.Empty,
                Description ?? string.Empty,
                (int)Access,
                Password ?? string.Empty,
                MaxVisitors,
                (int)Category,
                AllowPets ? 1 : 0,
                // @TODO Structure
                0, 0, 0, 0, ""
            );
        }
    }
}
