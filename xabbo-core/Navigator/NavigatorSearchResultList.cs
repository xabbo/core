using System;
using System.Collections.Generic;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class NavigatorSearchResultList : List<RoomInfo>
    {
        public static NavigatorSearchResultList Parse(Packet packet) => new NavigatorSearchResultList(packet);

        public string Category { get; set; }
        public string Text { get; set; }
        public int ActionAllowed { get; set; }
        public bool BoolA { get; set; }
        public int ViewMode { get; set; }

        public NavigatorSearchResultList() { }

        internal NavigatorSearchResultList(Packet packet)
        {
            Category = packet.ReadString();
            Text = packet.ReadString();
            ActionAllowed = packet.ReadInt();
            BoolA = packet.ReadBool();
            ViewMode = packet.ReadInt();
            int n = packet.ReadInt();
            for (int i = 0; i < n; i++)
                Add(RoomInfo.Parse(packet));
        }
    }
}
