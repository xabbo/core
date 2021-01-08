using System;
using System.Collections.Generic;

namespace Xabbo.Core
{
    public interface IHighScoreData : IItemData, IReadOnlyList<IHighScore> { }
}
