using System;
using System.Collections;
using System.Collections.Generic;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class HighScoreData : StuffData, IHighScoreData, IList<HighScoreData.HighScore>
    {
        private readonly List<HighScore> list;

        public int Int1 { get; set; }
        public int Int2 { get; set; }

        public int Count => list.Count;
        public bool IsReadOnly => false;
        public HighScore this[int index]
        {
            get => list[index];
            set => list[index] = value;
        }
        IHighScore IReadOnlyList<IHighScore>.this[int index] => this[index];
        IEnumerator<IHighScore> IEnumerable<IHighScore>.GetEnumerator() => GetEnumerator();

        public HighScoreData()
            : base(StuffDataType.HighScore)
        {
            list = new List<HighScore>();
        }

        protected override void Initialize(IReadOnlyPacket packet)
        {
            Value = packet.ReadString();
            Int1 = packet.ReadInt();
            Int2 = packet.ReadInt();

            short n = packet.ReadShort();
            for (int i = 0; i < n; i++)
            {
                var highScore = new HighScore();
                highScore.Value = packet.ReadInt(); 

                short k = packet.ReadShort();
                for (int j = 0; j < k; j++)
                    highScore.Names.Add(packet.ReadString());

                Add(highScore);
            }

            // No base call.
        }

        public class HighScore : IHighScore
        {
            public int Value { get; set; }
            public List<string> Names { get; set; }
            IReadOnlyList<string> IHighScore.Names => Names;

            public HighScore()
            {
                Names = new List<string>();
            }

            public void Write(IPacket packet)
            {
                packet.WriteInt(Value);
                packet.WriteShort((short)(Names?.Count ?? 0));
                if (Names != null)
                {
                    foreach (string name in Names)
                        packet.WriteString(name);
                }
            }
        }

        protected override void WriteData(IPacket packet)
        {
            packet.WriteString(Value);
            packet.WriteInt(Int1);
            packet.WriteInt(Int2);

            packet.WriteShort((short)Count);
            foreach (var highScore in this)
                highScore.Write(packet);
        }

        public int IndexOf(HighScore item) => list.IndexOf(item);
        public void Insert(int index, HighScore item) => list.Insert(index, item);
        public void RemoveAt(int index) => list.RemoveAt(index);
        public void Add(HighScore item) => list.Add(item);
        public void Clear() => list.Clear();
        public bool Contains(HighScore item) => list.Contains(item);
        public void CopyTo(HighScore[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);
        public bool Remove(HighScore item) => list.Remove(item);
        public IEnumerator<HighScore> GetEnumerator() => list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();
    }
}
