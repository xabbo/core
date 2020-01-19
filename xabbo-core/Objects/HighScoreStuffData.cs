using System;
using System.Collections.Generic;
using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class HighScoreStuffData : StuffData
    {
        public int UnknownIntA { get; private set; }
        public int UnknownIntB { get; private set; }

        private List<HighScore> scores;
        public IReadOnlyList<HighScore> Values { get; }

        public HighScoreStuffData()
            : base(StuffDataType.HighScoreStuffData)
        {
            scores = new List<HighScore>();
            Values = scores.AsReadOnly();
        }

        protected override void Initialize(Packet packet)
        {
            LegacyString = packet.ReadString();
            UnknownIntA = packet.ReadInteger();
            UnknownIntB = packet.ReadInteger();

            int n = packet.ReadInteger();
            for (int i = 0; i < n; i++)
            {
                HighScore highScore;

                highScore.Score = packet.ReadInteger(); 

                int n2 = packet.ReadInteger();
                string[] names = new string[n2];
                for (int j = 0; j < n2; j++)
                    names[j] = packet.ReadString();

                highScore.Names = names;

                scores.Add(highScore);
            }

            // No base call.
        }

        public struct HighScore
        {
            public int Score;
            public string[] Names;
        }

        protected override void WriteData(Packet packet)
        {
            packet.WriteString(LegacyString);
            packet.WriteInteger(UnknownIntA);
            packet.WriteInteger(UnknownIntB);

            packet.WriteInteger(Values.Count);
            foreach (var highScore in Values)
            {
                packet.WriteInteger(highScore.Score);
                packet.WriteInteger(highScore.Names.Length);
                foreach (string name in highScore.Names)
                    packet.WriteString(name);
            }
        }
    }
}
