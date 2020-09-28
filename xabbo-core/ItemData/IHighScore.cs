using System;
using System.Collections.Generic;

namespace Xabbo.Core
{
    public interface IHighScore
    {
        int Score { get; }
        IReadOnlyList<string> Names { get; }
    }
}
