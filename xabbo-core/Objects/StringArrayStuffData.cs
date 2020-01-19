using System;
using System.Collections;
using System.Collections.Generic;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class StringArrayStuffData : StuffData, IList<string>
    {
        private List<string> strings;

        public int Count => strings.Count;
        public bool IsReadOnly => false;

        public string this[int index]
        {
            get => strings[index];
            set => strings[index] = value;
        }

        public StringArrayStuffData()
            : base(StuffDataType.StringArrayStuffData)
        {
            strings = new List<string>();
        }

        protected override void Initialize(Packet packet)
        {
            int n = packet.ReadInteger();
            for (int i = 0; i < n; i++)
                strings.Add(packet.ReadString());

            base.Initialize(packet);
        }

        protected override void WriteData(Packet packet)
        {
            packet.WriteInteger(strings.Count);
            foreach (string value in strings)
                packet.WriteString(value);

            WriteBase(packet);
        }

        public int IndexOf(string item) => strings.IndexOf(item);
        public void Insert(int index, string item) => strings.Insert(index, item);
        public void RemoveAt(int index) => strings.RemoveAt(index);
        public void Add(string item) => strings.Add(item);
        public void Clear() => strings.Clear();
        public bool Contains(string item) => strings.Contains(item);
        public void CopyTo(string[] array, int arrayIndex) => strings.CopyTo(array, arrayIndex);
        public bool Remove(string item) => strings.Remove(item);
        public IEnumerator<string> GetEnumerator() => strings.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
