using System.Collections.Generic;

namespace Xabbo.Core;

/// <summary>
/// Associates a high score value with a list of users.
/// </summary>
public interface IHighScore
{
    /// <summary>
    /// The value of the high score.
    /// </summary>
    int Score { get; }

    /// <summary>
    /// The list of users with this score.
    /// </summary>
    IReadOnlyList<string> Users { get; }
}
