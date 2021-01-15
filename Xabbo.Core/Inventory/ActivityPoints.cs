using System;
using System.Collections;
using System.Collections.Generic;

using Xabbo.Core.Messages;

namespace Xabbo.Core
{
    public class ActivityPoints : IReadOnlyDictionary<ActivityPointType, int>
    {
        public static ActivityPoints Parse(IReadOnlyPacket packet) => new ActivityPoints(packet);

        private readonly Dictionary<ActivityPointType, int> dictionary = new Dictionary<ActivityPointType, int>();

        public IEnumerable<ActivityPointType> Keys => dictionary.Keys;
        public IEnumerable<int> Values => dictionary.Values;
        public int Count => dictionary.Count;

        public int this[ActivityPointType key]
        {
            get
            {
                lock (dictionary)
                {
                    return dictionary[key];
                }
            }

            set
            {
                lock (dictionary)
                {
                    dictionary[key] = value;
                }
            }
        }

        public ActivityPoints() { }

        protected ActivityPoints(IReadOnlyPacket packet)
        {
            int n = packet.ReadInt();
            for (int i = 0; i < n; i++)
            {
                var type = (ActivityPointType)packet.ReadInt();
                dictionary[type] = packet.ReadInt();
            }
        }

        public bool ContainsKey(ActivityPointType key) => dictionary.ContainsKey(key);

        public IEnumerator<KeyValuePair<ActivityPointType, int>> GetEnumerator() => dictionary.GetEnumerator();
        public bool TryGetValue(ActivityPointType key, out int value) => dictionary.TryGetValue(key, out value);
        IEnumerator IEnumerable.GetEnumerator() => dictionary.GetEnumerator();
    }
}
