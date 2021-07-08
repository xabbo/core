using System;

using Xabbo.Messages;

/*
	Achievement {
		int achievementId
		int level // level 0 must start at 1
		string string1
		int baseProgress
		int maxProgress = max(1, readInt())
			get() => maxProgress - baseProgress;
		int int3
		int int4
		int currentProgress
			get() => currentProgress - baseProgress;
		bool bool1 // Maxed?
		string category
		string string2
		int maxLevel
		int int7
		
		local bool ? => level > 1 || bool1;
	}
*/

namespace Xabbo.Core
{
    public class Achievement : IAchievement, IComposable
    {
        public int Id { get; set; }
        public int Level { get; set; }
        public string BadgeName { get; set; }
        public int BaseProgress { get; set; }
        public int MaxProgress { get; set; }
        public int Int5 { get; set; }
        public int Int6 { get; set; }
        public int CurrentProgress { get; set; }
        public bool IsCompleted { get; set; }
        public string Category { get; set; }
        public string String3 { get; set; }
        public int MaxLevel { get; set; }
        public int Int9 { get; set; }
        public short _Short1 { get; set; }

        public Achievement()
        {
            BadgeName = string.Empty;
            Category = string.Empty;
            String3 = string.Empty;
        }

        protected Achievement(IReadOnlyPacket packet)
        {
            Id = packet.ReadInt();
            Level = packet.ReadInt();
            BadgeName = packet.ReadString();
            BaseProgress = packet.ReadInt();
            MaxProgress = packet.ReadInt();
            Int5 = packet.ReadInt();
            Int6 = packet.ReadInt();
            CurrentProgress = packet.ReadInt();
            IsCompleted = packet.ReadBool();
            Category = packet.ReadString();
            String3 = packet.ReadString();
            MaxLevel = packet.ReadInt();
            Int9 = packet.ReadInt();
            _Short1 = packet.ReadShort();
        }

        public void Compose(IPacket packet)
        {
            packet.WriteValues(
                Id,
                Level,
                BadgeName,
                BaseProgress,
                MaxProgress,
                Int5,
                Int6,
                CurrentProgress,
                IsCompleted,
                Category,
                String3,
                MaxLevel,
                Int9,
                _Short1
            );
        }

        public static Achievement Parse(IReadOnlyPacket packet) => new Achievement(packet);
    }
}
