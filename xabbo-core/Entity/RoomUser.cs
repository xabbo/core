using System;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class RoomUser : Entity, IRoomUser
    {
        public Gender Gender { get; set; }
        public int GroupId { get; set; }
        public int Int8 { get; set; }
        public string GroupName { get; set; } 
        public string FigureExtra { get; set; }
        public int AchievementScore { get; set; }
        public bool Bool1 { get; set; }

        public int RightsLevel => CurrentUpdate?.ControlLevel ?? 0;
        public bool HasRights => RightsLevel > 0;

        public RoomUser(int id, int index)
            : base(EntityType.User, id, index)
        {
            Gender = Gender.Unisex;
            GroupId = -1;
            GroupName = "";
            FigureExtra = "";
        }

        internal RoomUser(int id, int index, IReadOnlyPacket packet)
            : this(id, index)
        {
            Gender = H.ToGender(packet.ReadString());
            GroupId = packet.ReadInt();
            Int8 = packet.ReadInt();
            GroupName = packet.ReadString();
            FigureExtra = packet.ReadString();
            AchievementScore = packet.ReadInt();
            Bool1 = packet.ReadBool();
        }

        protected override void OnUpdate(EntityUpdate update) { }

        public override void Write(IPacket packet)
        {
            base.Write(packet);

            packet.WriteValues(
                Gender.ToShortString(),
                GroupId,
                Int8,
                GroupName,
                FigureExtra,
                AchievementScore,
                Bool1
            );
        }
    }
}
