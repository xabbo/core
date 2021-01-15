using System;
using System.Collections;
using System.Collections.Generic;

using Xabbo.Core.Messages;

namespace Xabbo.Core
{
    public class MapData : StuffData, IMapData, IDictionary<string, string>
    {
        private readonly Dictionary<string, string> dict;

        public ICollection<string> Keys => dict.Keys;
        IEnumerable<string> IReadOnlyDictionary<string, string>.Keys => Keys;
        public ICollection<string> Values => dict.Values;
        IEnumerable<string> IReadOnlyDictionary<string, string>.Values => Values;
        public int Count => dict.Count;
        public bool IsReadOnly => false;

        public string this[string key]
        {
            get => dict[key];
            set => dict[key] = value;
        }

        public MapData()
            : base(StuffDataType.Map)
        {
            dict = new Dictionary<string, string>();
        }

        protected override void Initialize(IReadOnlyPacket packet)
        {
            short n = packet.ReadShort();
            for (int i = 0; i < n; i++)
                dict.Add(packet.ReadString(), packet.ReadString());

            base.Initialize(packet);
        }

        protected override void WriteData(IPacket packet)
        {
            packet.WriteShort((short)dict.Count);
            foreach (var item in dict)
            {
                packet.WriteString(item.Key);
                packet.WriteString(item.Value);
            }

            WriteBase(packet);
        }

        public bool ContainsKey(string key) => dict.ContainsKey(key);
        public void Add(string key, string value) => dict.Add(key, value);
        public bool Remove(string key) => dict.Remove(key);
        public bool TryGetValue(string key, out string value) => TryGetValue(key, out value!);
        public void Add(KeyValuePair<string, string> item) => ((IDictionary<string, string>)dict).Add(item);
        public void Clear() => dict.Clear();
        public bool Contains(KeyValuePair<string, string> item) => ((IDictionary<string, string>)dict).Contains(item);
        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex) => ((IDictionary<string, string>)dict).CopyTo(array, arrayIndex);
        public bool Remove(KeyValuePair<string, string> item) => ((IDictionary<string, string>)dict).Remove(item);
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => dict.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
