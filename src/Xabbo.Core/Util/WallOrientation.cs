using System;

namespace Xabbo.Core;

/// <summary>
/// Represents a wall orientation.
/// </summary>
public readonly record struct WallOrientation
{
    /// <summary>
    /// Represents a left wall orientation.
    /// </summary>
    public static readonly WallOrientation Left = new('l');

    /// <summary>
    /// Represents a left wall orientation.
    /// </summary>
    public static readonly WallOrientation Right = new('r');

    /// <summary>
    /// Gets the character value of this wall orientation.
    /// </summary>
    public readonly char Value;

    /// <summary>
    /// Gets if this is a left wall orientation.
    /// </summary>
    public bool IsLeft => Value == 'l';

    /// <summary>
    /// Gets if this is a right wall orientation.
    /// </summary>
    public bool IsRight => Value == 'r';

    private WallOrientation(char value)
    {
        Value = value;
    }

    public WallOrientation Opposite => IsLeft ? Right : Left;

    public override string ToString() => Value.ToString();

    /// <summary>
    /// Gets the wall orientation by the specified character. Must be <c>l</c> or <c>r</c>.
    /// </summary>
    /// <exception cref="ArgumentException">The specified character is not a valid wall orientation.</exception>
    public static WallOrientation FromChar(char c) => c switch
    {
        'l' => Left,
        'r' => Right,
        _ => throw new ArgumentException($"Invalid wall orientation '{c}'. Must be 'l' or 'r'."),
    };

    public static implicit operator WallOrientation(char c) => FromChar(c);
    public static implicit operator char(WallOrientation orientation) => orientation.Value;
    public static implicit operator string(WallOrientation orientation) => orientation.ToString();
}
