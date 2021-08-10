using System;
using System.Collections.Generic;

namespace Xabbo.Core
{
    /// <summary>
    /// Defines high score information as extra data in an item.
    /// </summary>
    public interface IHighScoreData : IItemData, IReadOnlyList<IHighScore>
    {
        int ScoreType { get; }
        int ClearType { get; }
    }
}
