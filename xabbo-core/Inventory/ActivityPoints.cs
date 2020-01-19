using System;
using System.Collections;
using System.Collections.Generic;


using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class ActivityPoints
    {
        private readonly Dictionary<ActivityPointType, int> dictionary = new Dictionary<ActivityPointType, int>();

        public ActivityPoints() { }

        internal ActivityPoints(Packet packet)
        {
            int n = packet.ReadInteger();
            for (int i = 0; i < n; i++)
            {
                var type = (ActivityPointType)packet.ReadInteger();
                dictionary[type] = packet.ReadInteger();
            }
        }

        public int this[ActivityPointType key]
        {
            get
            {
                lock (dictionary)
                {
                    if (dictionary.TryGetValue(key, out int value)) return value;
                    else return 0;
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

        public static ActivityPoints Parse(Packet packet) => new ActivityPoints(packet);
    }
}
