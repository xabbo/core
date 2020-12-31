using System;
using System.Collections.Generic;

namespace Xabbo.Core
{
    public interface IAchievements : IReadOnlyCollection<IAchievement>
    {
        IAchievement? this[int id] { get; }
    }
}
