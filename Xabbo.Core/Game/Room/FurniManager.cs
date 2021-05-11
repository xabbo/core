using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using Xabbo.Messages;
using Xabbo.Interceptor;

using Xabbo.Core.Events;

namespace Xabbo.Core.Game
{
    public class FurniManager : GameStateManager
    {
        private readonly RoomManager _roomManager;

        private readonly ConcurrentDictionary<long, FloorItem> _floorItems = new();
        private readonly ConcurrentDictionary<long, WallItem> _wallItems = new();

        public IEnumerable<IFloorItem> FloorItems => _floorItems.Select(item => item.Value);
        public IEnumerable<IWallItem> WallItems => _wallItems.Select(item => item.Value);
        public IEnumerable<IFurni> Furni => FloorItems.Concat<IFurni>(WallItems);

        

        

        public FurniManager(IInterceptor interceptor, RoomManager roomManager)
            : base(interceptor)
        {
            _roomManager = roomManager;
            _roomManager.Left += Room_Left;
        }

        private void Room_Left(object? sender, EventArgs e)
        {
            _floorItems.Clear();
            _wallItems.Clear();
        }

        [InterceptIn(nameof(Incoming.Ping))]
        private void DeleteThis(InterceptArgs e) { }

        #region - Interaction -
        public void Place(long itemId, int x, int y, int direction)
            => SendAsync(Out.PlaceRoomItem, itemId, x, y, direction);
        public void Place(long itemId, (int X, int Y) location, int direction)
            => SendAsync(Out.PlaceRoomItem, itemId, location.X, location.Y, direction);
        public void Place(IInventoryItem item, int x, int y, int direction)
            => Place(item.Id, x, y, direction);
        public void Place(IInventoryItem item, (int X, int Y) location, int direction)
            => Place(item.Id, location, direction);

        public void Place(long itemId, WallLocation location)
            => SendAsync(Out.PlaceWallItem, itemId, location.WallX, location.WallY, location.X, location.Y);
        public void Place(IInventoryItem item, WallLocation location)
            => Place(item.Id, location);

        public void Move(long floorItemId, int x, int y, int direction)
            => SendAsync(Out.MoveRoomItem, floorItemId, x, y, direction);
        public void Move(long floorItemId, (int X, int Y) location, int direction)
            => SendAsync(Out.MoveRoomItem, floorItemId, location.X, location.Y, direction);
        public void Move(IFloorItem item, int x, int y, int direction)
            => Move(item.Id, x, y, direction);
        public void Move(IFloorItem item, (int X, int Y) location, int direction)
            => Move(item.Id, location.X, location.Y, direction);

        public void Move(long wallItemId, WallLocation location) => SendAsync(
            Out.MoveWallItem, wallItemId,
            location.WallX, location.WallY,
            location.X, location.Y,
            location.Orientation.Value.ToString()
        );
        public void Move(IWallItem item, WallLocation location)
            => Move(item.Id, location);

        public void Pickup(IFurni item) => Pickup(item.Type, item.Id);
        public void Pickup(ItemType type, long id)
        {
            if (type == ItemType.Floor)
                SendAsync(Out.PickItemUpFromRoom, 2, id);
            else if (type == ItemType.Wall)
                SendAsync(Out.PickItemUpFromRoom, 1, id);
        }

        public void UpdateStackTile(IFloorItem stackTile, float height) => UpdateStackTile(stackTile.Id, height);
        public void UpdateStackTile(long stackTileId, float height)
            => SendAsync(Out.StackingHelperSetCaretHeight, stackTileId, (int)Math.Round(height * 100.0));
        #endregion
    }
}
