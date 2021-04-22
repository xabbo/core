using System;

using Xabbo.Messages;

namespace Xabbo.Core
{
    public class RoomUser : Entity, IRoomUser
    {
        public Gender Gender { get; set; }
        public long GroupId { get; set; }
        public int Int8 { get; set; }
        public string GroupName { get; set; } 
        public string FigureExtra { get; set; }
        public int AchievementScore { get; set; }
        public bool Bool1 { get; set; }

        public int RightsLevel => CurrentUpdate?.ControlLevel ?? 0;
        public bool HasRights => RightsLevel > 0;

        public RoomUser(long id, int index)
            : base(EntityType.User, id, index)
        {
            Gender = Gender.Unisex;
            GroupId = -1;
            GroupName = "";
            FigureExtra = "";
        }

        internal RoomUser(long id, int index, IReadOnlyPacket packet)
            : this(id, index)
        {
            Gender = H.ToGender(packet.ReadString());
            GroupId = packet.ReadLegacyLong();
            Int8 = packet.ReadInt();
            GroupName = packet.ReadString();
            FigureExtra = packet.ReadString();
            AchievementScore = packet.ReadInt();
            Bool1 = packet.ReadBool();
        }

        protected override void OnUpdate(EntityStatusUpdate update) { }

        public override void Compose(IPacket packet)
        {
            base.Compose(packet);

            packet
                .WriteString(Gender.ToShortString())
                .WriteLegacyLong(GroupId)
                .WriteInt(Int8)
                .WriteString(GroupName)
                .WriteString(FigureExtra)
                .WriteInt(AchievementScore)
                .WriteBool(Bool1);
        }
    }
}
