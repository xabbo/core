namespace Xabbo.Core;

public interface IFloorPlan
{
    /// <summary>
    /// Gets the original string that this floor plan was parsed from.
    /// </summary>
    string? OriginalString { get; }

    /// <summary>
    /// Gets the wall scaling for this floor plan.
    /// </summary>
    int Scale { get; }

    /// <summary>
    /// Gets the wall height for this floor plan.
    /// </summary>
    int WallHeight { get; }

    /// <summary>
    /// Gets the size of the floor plan.
    /// </summary>
    Point Size { get; }

    /// <summary>
    /// Gets the area of the floor plan.
    /// </summary>
    Area Area { get; }

    /// <summary>
    /// Gets the tile height at the specified coordinates.
    /// </summary>
    int this[int x, int y] { get; }

    /// <summary>
    /// Gets the tile height at the specified coordinates.
    /// </summary>
    int this[Point point] { get; }

    /// <summary>
    /// Gets if the tile at the specified coordinates in this floor plan is walkable.
    /// </summary>
    bool IsWalkable(Point point);

    /// <summary>
    /// Gets if the tile at the specified coordinates in this floor plan is walkable.
    /// </summary>
    bool IsWalkable(int x, int y);
}
