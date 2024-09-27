using System.Collections;
using System.Collections.Generic;

namespace Xabbo.Core;

/// <summary>
/// Defines the standard sticky note colors.
/// </summary>
public sealed class StickyColors : IReadOnlyList<StickyColor>
{
    public static readonly StickyColor Blue = new("Blue", "9CCEFF");
    public static readonly StickyColor Pink = new("Pink", "FF9CFF");
    public static readonly StickyColor Green = new("Green", "9CFF9C");
    public static readonly StickyColor Yellow = new("Yellow", "FFFF33");

    public static readonly StickyColors All = new();

    private readonly List<StickyColor> colors;

    internal StickyColors()
    {
        colors = new List<StickyColor>()
        {
            Blue,
            Pink,
            Green,
            Yellow
        };
    }

    public StickyColor this[int index] => colors[index];
    public int Count => colors.Count;
    public IEnumerator<StickyColor> GetEnumerator() => colors.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
