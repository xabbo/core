using System;
using System.Collections.Generic;
using System.Linq;

namespace Xabbo.Core.Game
{
    public interface IRoom
    {
        long Id { get; }

        IRoomData? Data { get; }

        string Model { get; }
        string Floor { get; }
        string Wallpaper { get; }
        string Landscape { get; }

        Tile DoorTile { get; }
        int EntryDirection { get; }

        IFloorPlan FloorPlan { get; }
        IHeightmap Heightmap { get; }

        IEnumerable<IFurni> Furni => FloorItems.Concat<IFurni>(WallItems);
        IEnumerable<IFloorItem> FloorItems { get; }
        IEnumerable<IWallItem> WallItems { get; }

        IEnumerable<IEntity> Entities { get; }
    }
}
