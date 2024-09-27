using System;
using System.Collections.Generic;

namespace Xabbo.Core;

/// <summary>
/// Represents a list of high scores.
/// </summary>
public interface IHighScoreData : IItemData, IReadOnlyList<IHighScore>
{
    int ScoreType { get; }
    int ClearType { get; }
}
