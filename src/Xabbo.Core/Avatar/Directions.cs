namespace Xabbo.Core;

/// <summary>
/// Represents a direction in a room.
/// </summary>
/// <remarks>
/// Values range from 0-7, with each increment representing a 45-degree turn in the clockwise direction.
/// The first value (North) points towards the negative Y axis. (<c>↗</c>)
/// </remarks>
public enum Directions
{
    /// <summary>
    /// Represents the north direction.
    /// </summary>
    /// <remarks>
    /// Points towards the negative Y axis. (<c>↗</c>)
    /// </remarks>
    North = 0,
    /// <summary>
    /// Represents the north-east direction.
    /// </summary>
    /// <remarks>
    /// Points towards the positive X, negative Y axis. (<c>→</c>)
    /// </remarks>
    NorthEast = 1,
    /// <summary>
    /// Represents the east direction.
    /// </summary>
    /// <remarks>
    /// Points towards the positive X axis. (<c>↘</c>)
    /// </remarks>
    East = 2,
    /// <summary>
    /// Represents the south-east direction.
    /// </summary>
    /// <remarks>
    /// Points towards the positive X, positive Y axis. (<c>↓</c>)
    /// </remarks>
    SouthEast = 3,
    /// <summary>
    /// Represents the south direction.
    /// </summary>
    /// <remarks>
    /// Points towards the positive Y axis. (<c>↙</c>)
    /// </remarks>
    South = 4,
    /// <summary>
    /// Represents the south-west direction.
    /// </summary>
    /// <remarks>
    /// Points towards the negative X, positive Y axis. (<c>←</c>)
    /// </remarks>
    SouthWest = 5,
    /// <summary>
    /// Represents the west direction.
    /// </summary>
    /// <remarks>
    /// Points towards the negative X axis. (<c>↖</c>)
    /// </remarks>
    West = 6,
    /// <summary>
    /// Represents the north-west direction.
    /// </summary>
    /// <remarks>
    /// Points towards the negative X, negative Y axis. (<c>↑</c>)
    /// </remarks>
    NorthWest = 7
}
