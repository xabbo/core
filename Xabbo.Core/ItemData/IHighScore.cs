using System;
using System.Collections.Generic;
using Xabbo.Messages;

namespace Xabbo.Core
{
    public interface IHighScore : IComposable
    {
        int Value { get; }
        IReadOnlyList<string> Names { get; }
    }
}
