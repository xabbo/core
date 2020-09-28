using System;

namespace Xabbo.Core
{
    public interface IFloorItem : IFurni
    {
        ITile Location { get; }
        int X { get; }
        int Y { get; }
        double Z { get; }
        int Direction { get; }
        double Height { get; }
        int Extra { get;  }

        IItemData Data { get; }
    }
}
