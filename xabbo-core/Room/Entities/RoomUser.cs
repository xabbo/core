using System;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class RoomUser : Entity
    {
        public Gender Gender { get; set; }
        public int GroupId { get; set; }
        public int UnknownIntA { get; set; }
        public string GroupName { get; set; } 
        public string FigureExtra { get; set; }
        public int AchievementScore { get; set; }
        public bool UnknownBoolA { get; set; }

        // Extra state
        public int RightsLevel
        {
            get
            {
                var update = CurrentUpdate;
                if (update != null)
                    return update.ControlLevel;
                else
                    return 0;
            }
        }
        public bool HasRights => RightsLevel > 0;

        public RoomUser(int id, int index)
            : base(EntityType.User, id, index)
        { }

        internal RoomUser(int id, int index, Packet packet)
            : this(id, index)
        {
            Gender = H.ToGender(packet.ReadString());
            GroupId = packet.ReadInteger();
            UnknownIntA = packet.ReadInteger();
            GroupName = packet.ReadString();
            FigureExtra = packet.ReadString();
            AchievementScore = packet.ReadInteger();
            UnknownBoolA = packet.ReadBoolean();
        }

        protected override void OnUpdate(EntityUpdate update) { }

        public override void Write(Packet packet)
        {
            base.Write(packet);

            packet.WriteValues(
                Gender.ToShortString(),
                GroupId,
                UnknownIntA,
                GroupName,
                FigureExtra,
                AchievementScore,
                UnknownBoolA
            );
        }
    }
}
