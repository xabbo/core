using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core;

public class NavigatorSearchResultList : List<RoomInfo>, IComposer, IParser<NavigatorSearchResultList>
{
    public static NavigatorSearchResultList Parse(in PacketReader packet) => new NavigatorSearchResultList(in packet);

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

    protected NavigatorSearchResultList(in PacketReader p)
    {
        Category = p.Read<string>();
        Text = p.Read<string>();
        ActionAllowed = p.Read<int>();
        ForceClosed = p.Read<bool>();
        ViewMode = p.Read<int>();
        int n = p.Read<Length>();
        for (int i = 0; i < n; i++)
            Add(p.Parse<RoomInfo>());
    }

    public void Compose(in PacketWriter p)
    {
        p.Write(Category);
        p.Write(Text);
        p.Write(ActionAllowed);
        p.Write(ForceClosed);
        p.Write(ViewMode);
        p.Write<Length>(Count);
        for (int i = 0; i < Count; i++)
            p.Write(this[i]);
    }
}
