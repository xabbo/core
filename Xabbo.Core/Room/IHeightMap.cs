using System;
using System.Collections.Generic;
using Xabbo.Messages;

namespace Xabbo.Core
{
    public interface IHeightmap : IComposable
    {
        int Width { get; }
        int Length { get; }
        IReadOnlyList<short> Values { get; }
        short this[int x, int y] { get; }
        double GetHeight(int x, int y);
        bool IsBlocked(int x, int y);
        bool IsTile(int x, int y);
        bool IsFree(int x, int y);
    }
}
