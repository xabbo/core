using System;
using System.Collections;
using System.Collections.Generic;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class IntArrayData : StuffData, IIntArrayData, IList<int>
    {
        private readonly List<int> list;

        public int Count => list.Count;
        public bool IsReadOnly => false;
        public int this[int index]
        {
            get => list[index];
            set => list[index] = value;
        }

        public IntArrayData()
            : base(StuffDataType.IntArray)
        {
            list = new List<int>();
        }

        protected override void Initialize(IReadOnlyPacket packet)
        {
            short n = packet.ReadShort();
            for (int i = 0; i < n; i++)
                list.Add(packet.ReadInt());

            base.Initialize(packet);
        }

        protected override void WriteData(IPacket packet)
        {
            packet.WriteShort((short)Count);
            foreach (int value in this)
                packet.WriteInt(value);

            WriteBase(packet);
        }

        public int IndexOf(int item) => list.IndexOf(item);
        public void Insert(int index, int item) => list.Insert(index, item);
        public void RemoveAt(int index) => list.RemoveAt(index);
        public void Add(int item) => list.Add(item);
        public void Clear() => list.Clear();
        public bool Contains(int item) => list.Contains(item);
        public void CopyTo(int[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);
        public bool Remove(int item) => list.Remove(item);
        public IEnumerator<int> GetEnumerator() => list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();
    }
}
