using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core;

/// <summary>
/// Represents a list of <see cref="RoomInfo"/>.
/// </summary>
public class NavigatorSearchResultList : List<RoomInfo>, IParserComposer<NavigatorSearchResultList>
{
    public string Category { get; set; }
    public string Text { get; set; }
    public int ActionAllowed { get; set; }
    public bool ForceClosed { get; set; }
    public int ViewMode { get; set; }

    public NavigatorSearchResultList()
    {
        Category =
        Text = "";
    }

    protected NavigatorSearchResultList(in PacketReader p)
    {
        Category = p.ReadString();
        Text = p.ReadString();
        ActionAllowed = p.ReadInt();
        ForceClosed = p.ReadBool();
        ViewMode = p.ReadInt();
        AddRange(p.ParseArray<RoomInfo>());
    }

    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteString(Category);
        p.WriteString(Text);
        p.WriteInt(ActionAllowed);
        p.WriteBool(ForceClosed);
        p.WriteInt(ViewMode);
        p.ComposeArray(this);
    }

    static NavigatorSearchResultList IParser<NavigatorSearchResultList>.Parse(in PacketReader p) => new(in p);
}
