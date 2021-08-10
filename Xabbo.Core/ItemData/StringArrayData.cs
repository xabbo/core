using System;
using System.Collections;
using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core
{
    public class StringArrayData : ItemData, IStringArrayData, IList<string>
    {
        private readonly List<string> _list;

        public int Count => _list.Count;
        bool ICollection<string>.IsReadOnly => false;

        public string this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        public StringArrayData()
            : base(ItemDataType.StringArray)
        {
            _list = new List<string>();
        }

        public StringArrayData(IStringArrayData data)
            : base(data)
        {
            _list = new List<string>(data);
        }

        protected override void Initialize(IReadOnlyPacket packet)
        {
            short n = packet.ReadLegacyShort();
            for (int i = 0; i < n; i++)
            {
                _list.Add(packet.ReadString());
            }

            base.Initialize(packet);
        }

        protected override void WriteData(IPacket packet)
        {
            packet.WriteLegacyShort((short)_list.Count);
            foreach (string value in _list)
            {
                packet.WriteString(value);
            }

            WriteBase(packet);
        }

        public int IndexOf(string item) => _list.IndexOf(item);
        public void Insert(int index, string item) => _list.Insert(index, item);
        public void RemoveAt(int index) => _list.RemoveAt(index);
        public void Add(string item) => _list.Add(item);
        public void Clear() => _list.Clear();
        public bool Contains(string item) => _list.Contains(item);
        public void CopyTo(string[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);
        public bool Remove(string item) => _list.Remove(item);
        public IEnumerator<string> GetEnumerator() => _list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
