using System;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class RoomUser : Entity
    {
        public Gender Gender { get; set; }
        public int GroupId { get; set; }
        public int Unknown1 { get; set; }
        public string GroupName { get; set; } 
        public string FigureExtra { get; set; }
        public int AchievementScore { get; set; }
        public bool Unknown2 { get; set; }

        // Extra state
        public int RightsLevel { get; set; }
        public bool HasRights => RightsLevel > 0;

        public RoomUser(int id, int index)
            : base(EntityType.User, id, index)
        { }

        internal RoomUser(int id, int index, Packet packet)
            : this(id, index)
        {
            Gender = H.ToGender(packet.ReadString());
            GroupId = packet.ReadInteger();
            Unknown1 = packet.ReadInteger();
            GroupName = packet.ReadString();
            FigureExtra = packet.ReadString();
            AchievementScore = packet.ReadInteger();
            Unknown2 = packet.ReadBoolean();
        }

        protected override void OnUpdate(EntityUpdate update)
        {
            if (update.IsController && (RightsLevel != update.ControlLevel))
                RightsLevel = update.ControlLevel;
            else if (!update.IsController && RightsLevel != 0)
                RightsLevel = 0;
        }
    }
}
