using System;
using System.Collections.Generic;
using Xabbo.Messages;

namespace Xabbo.Core;

public class NavigatorSearchResultList : List<RoomInfo>
{
    public static NavigatorSearchResultList Parse(IReadOnlyPacket packet) => new NavigatorSearchResultList(packet);

    public string Category { get; set; }
    public string Text { get; set; }
    public int ActionAllowed { get; set; }
    public bool ForceClosed { get; set; }
    public int ViewMode { get; set; }

    public NavigatorSearchResultList()
    {
        Category =
        Text = string.Empty;
    }

    protected NavigatorSearchResultList(IReadOnlyPacket packet)
    {
        Category = packet.ReadString();
        Text = packet.ReadString();
        ActionAllowed = packet.ReadInt();
        ForceClosed = packet.ReadBool();
        ViewMode = packet.ReadInt();
        short n = packet.ReadLegacyShort();
        for (int i = 0; i < n; i++)
            Add(RoomInfo.Parse(packet));
    }
}
