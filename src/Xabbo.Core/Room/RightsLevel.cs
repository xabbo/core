namespace Xabbo.Core;

/// <summary>
/// Represents a room rights level.
/// </summary>
public enum RightsLevel
{
    /// <summary>
    /// Represents no rights.
    /// </summary>
    None = 0,
    /// <summary>
    /// Represents standard rights.
    /// </summary>
    Standard = 1,
    /// <summary>
    /// Represents a group administrator's rights.
    /// </summary>
    GroupAdmin = 3,
    /// <summary>
    /// Represents the room owner's rights.
    /// </summary>
    Owner = 4,
}
