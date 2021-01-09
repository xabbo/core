using System;
using System.Collections;
using System.Collections.Generic;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class StringArrayData : StuffData, IStringArrayData, IList<string>
    {
        private readonly List<string> list;

        public int Count => list.Count;
        public bool IsReadOnly => false;
        public string this[int index]
        {
            get => list[index];
            set => list[index] = value;
        }

        public StringArrayData()
            : base(StuffDataType.StringArray)
        {
            list = new List<string>();
        }

        protected override void Initialize(IReadOnlyPacket packet)
        {
            short n = packet.ReadShort();
            for (int i = 0; i < n; i++)
                list.Add(packet.ReadString());

            base.Initialize(packet);
        }

        protected override void WriteData(IPacket packet)
        {
            packet.WriteShort((short)list.Count);
            foreach (string value in list)
                packet.WriteString(value);

            WriteBase(packet);
        }

        public int IndexOf(string item) => list.IndexOf(item);
        public void Insert(int index, string item) => list.Insert(index, item);
        public void RemoveAt(int index) => list.RemoveAt(index);
        public void Add(string item) => list.Add(item);
        public void Clear() => list.Clear();
        public bool Contains(string item) => list.Contains(item);
        public void CopyTo(string[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);
        public bool Remove(string item) => list.Remove(item);
        public IEnumerator<string> GetEnumerator() => list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
