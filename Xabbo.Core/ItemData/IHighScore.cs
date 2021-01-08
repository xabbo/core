using System;
using System.Collections.Generic;
using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public interface IHighScore : IPacketData
    {
        int Value { get; }
        IReadOnlyList<string> Names { get; }
    }
}
