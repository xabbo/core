namespace Xabbo.Core;

/// <summary>
/// Represents a sticky color.
/// </summary>
public class StickyColor(string name, string value)
{
    public string Name { get; } = name;
    public string Value { get; } = value;

    public static implicit operator string(StickyColor color) => color.Value;
}
