using System;

using Xabbo.Messages;

namespace Xabbo.Core
{
    /*
        int achievementId
        int level
        string badgeId
        int scoreAtStartOfLevel
        int scoreLimit
          -> max(1, value) - scoreAtStartOfLevel
        int levelRewardPoints
        int levelRewardPointType
        int currentPoints
          -> value - scoreAtStartOfLevel
        bool finalLevel
        string category
        string subCategory
        int levelCount
        int displayMethod
        short state

        firstLevelAchieved = level > 1 || finalLevel
     */

    public class Achievement : IAchievement, IComposable
    {
        public int Id { get; set; }
        public int Level { get; set; }
        public string BadgeId { get; set; }
        public int BaseProgress { get; set; }
        public int MaxProgress { get; set; }
        public int LevelRewardPoints { get; set; }
        public int LevelRewardPointType { get; set; }
        public int CurrentProgress { get; set; }
        public bool IsComplete { get; set; }
        public string Category { get; set; }
        public string Subcategory { get; set; }
        public int MaxLevel { get; set; }
        public int DisplayMethod { get; set; }
        public short State { get; set; }

        public Achievement()
        {
            BadgeId = string.Empty;
            Category = string.Empty;
            Subcategory = string.Empty;
        }

        protected Achievement(IReadOnlyPacket packet)
        {
            Id = packet.ReadInt();
            Level = packet.ReadInt();
            BadgeId = packet.ReadString();
            BaseProgress = packet.ReadInt();
            MaxProgress = packet.ReadInt();
            LevelRewardPoints = packet.ReadInt();
            LevelRewardPointType = packet.ReadInt();
            CurrentProgress = packet.ReadInt();
            IsComplete = packet.ReadBool();
            Category = packet.ReadString();
            Subcategory = packet.ReadString();
            MaxLevel = packet.ReadInt();
            DisplayMethod = packet.ReadInt();
            State = packet.ReadShort();
        }

        public void Compose(IPacket packet)
        {
            packet
                .WriteInt(Id)
                .WriteInt(Level)
                .WriteString(BadgeId)
                .WriteInt(BaseProgress)
                .WriteInt(MaxProgress)
                .WriteInt(LevelRewardPoints)
                .WriteInt(LevelRewardPointType)
                .WriteInt(CurrentProgress)
                .WriteBool(IsComplete)
                .WriteString(Category)
                .WriteString(Subcategory)
                .WriteInt(MaxLevel)
                .WriteInt(DisplayMethod)
                .WriteShort(State);
        }

        public static Achievement Parse(IReadOnlyPacket packet) => new Achievement(packet);
    }
}
