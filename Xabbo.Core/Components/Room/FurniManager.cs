using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using Xabbo.Core.Events;
using Xabbo.Core.Messages;
using Xabbo.Core.Protocol;

namespace Xabbo.Core.Components
{
    [Dependencies(typeof(RoomManager))]
    public class FurniManager : XabboComponent
    {
        public enum Features
        {
            Management,
            FloorItemManagement,
            FloorItemUpdates,
            WallItemManagement,
            WallItemUpdates
        }

        private RoomManager roomManager;

        private readonly ConcurrentDictionary<long, FloorItem> _floorItems;
        private readonly ConcurrentDictionary<long, WallItem> _wallItems;

        public IEnumerable<IFloorItem> FloorItems => _floorItems.Select(item => item.Value);
        public IEnumerable<IWallItem> WallItems => _wallItems.Select(item => item.Value);
        public IEnumerable<IFurni> Furni => FloorItems.Concat<IFurni>(WallItems);

        /// <summary>
        /// Gets whether the floor item with the specified ID exists in the room or not.
        /// </summary>
        public bool FloorItemExists(long itemId) => _floorItems.ContainsKey(itemId);
        /// <summary>
        /// Gets whether the wall item with the specified ID exists in the room or not.
        /// </summary>
        public bool WallItemExists(long itemId) => _wallItems.ContainsKey(itemId);

        /// <summary>
        /// Gets the furni of the specified type with the specified ID, or <c>null</c> if it does not exist.
        /// </summary>
        public IFurni? GetFurni(ItemType type, long itemId)
        {
            if (type == ItemType.Floor) return GetFloorItem(itemId);
            else if (type == ItemType.Wall) return GetWallItem(itemId);
            else throw new InvalidOperationException($"ItemType must be Floor or Wall");
        }

        /// <summary>
        /// Gets the floor item with the specified ID or <c>null</c> if it does not exist.
        /// </summary>
        public IFloorItem? GetFloorItem(long itemId) => _floorItems.TryGetValue(itemId, out FloorItem? item) ? item : null;
        /// <summary>
        /// Gets the wall item with the specified ID or <c>null</c> if it does not exist.
        /// </summary>
        public IWallItem? GetWallItem(long itemId) => _wallItems.TryGetValue(itemId, out WallItem? item) ? item : null;

        #region - Events -
        /// <summary>
        /// Invoked when the floor items are loaded.
        /// This may happen multiple times depending on
        /// how many items are in the room.
        /// </summary>
        public event EventHandler<FloorItemsEventArgs>? FloorItemsLoaded;
        /// <summary>
        /// Invoked when a floor item is added to the room.
        /// </summary>
        public event EventHandler<FloorItemEventArgs>? FloorItemAdded;
        /// <summary>
        /// Invoked when a floor item is updated.
        /// </summary>
        public event EventHandler<FloorItemUpdatedEventArgs>? FloorItemUpdated;
        /// <summary>
        /// Invoked when a floor item's data is updated.
        /// </summary>
        public event EventHandler<FloorItemDataUpdatedEventArgs>? FloorItemDataUpdated;
        /// <summary>
        /// Invoked when a floor item slides due to a roller or wired update.
        /// </summary>
        public event EventHandler<FloorItemSlideEventArgs>? FloorItemSlide;
        /// <summary>
        /// Invoked when a floor item is removed from the room.
        /// </summary>
        public event EventHandler<FloorItemEventArgs>? FloorItemRemoved;

        /// <summary>
        /// Invoked when the wall items are loaded.
        /// This may happen multiple times depending on
        /// how many items are in the room.
        /// </summary>
        public event EventHandler<WallItemsEventArgs>? WallItemsLoaded;
        /// <summary>
        /// Invoked when a wall item is added to the room.
        /// </summary>
        public event EventHandler<WallItemEventArgs>? WallItemAdded;
        /// <summary>
        /// Invoked when a wall item is udpated.
        /// </summary>
        public event EventHandler<WallItemUpdatedEventArgs>? WallItemUpdated;
        /// <summary>
        /// Invoked when a wall item is removed from the room.
        /// </summary>
        public event EventHandler<WallItemEventArgs>? WallItemRemoved;

        /// <summary>
        /// Invoked when a furni's visibility is toggled using <see cref="Hide(IFurni)"/> or <see cref="Show(IFurni)"/>.
        /// </summary>
        public event EventHandler<FurniEventArgs>? FurniVisibilityToggled;

        protected virtual void OnFloorItemsLoaded(IEnumerable<IFloorItem> items)
            => FloorItemsLoaded?.Invoke(this, new FloorItemsEventArgs(items));
        protected virtual void OnFloorItemAdded(IFloorItem item)
            => FloorItemAdded?.Invoke(this, new FloorItemEventArgs(item));
        protected virtual void OnFloorItemUpdated(IFloorItem previousItem, IFloorItem updatedItem)
            => FloorItemUpdated?.Invoke(this, new FloorItemUpdatedEventArgs(previousItem, updatedItem));
        protected virtual void OnFloorItemDataUpdated(IFloorItem item, IItemData previousData)
            => FloorItemDataUpdated?.Invoke(this, new FloorItemDataUpdatedEventArgs(item, previousData));
        protected virtual void OnFloorItemSlide(IFloorItem item, Tile previousTile, int rollerId)
            => FloorItemSlide?.Invoke(this, new FloorItemSlideEventArgs(item, previousTile, rollerId));
        protected virtual void OnFloorItemRemoved(IFloorItem item)
            => FloorItemRemoved?.Invoke(this, new FloorItemEventArgs(item));

        protected virtual void OnWallItemsLoaded(IEnumerable<IWallItem> items)
            => WallItemsLoaded?.Invoke(this, new WallItemsEventArgs(items));
        protected virtual void OnWallItemAdded(IWallItem item)
            => WallItemAdded?.Invoke(this, new WallItemEventArgs(item));
        protected virtual void OnWallItemUpdated(IWallItem previousItem, IWallItem updatedItem)
            => WallItemUpdated?.Invoke(this, new WallItemUpdatedEventArgs(previousItem, updatedItem));
        protected virtual void OnWallItemRemoved(IWallItem item)
            => WallItemRemoved?.Invoke(this, new WallItemEventArgs(item));

        protected virtual void OnFurniVisibilityToggled(IFurni furni)
            => FurniVisibilityToggled?.Invoke(this, new FurniEventArgs(furni));
        #endregion

        public FurniManager()
        {
            _floorItems = new ConcurrentDictionary<long, FloorItem>();
            _wallItems = new ConcurrentDictionary<long, WallItem>();
        }

        protected override void OnInitialize()
        {
            // TODO DI ?
            roomManager = GetComponent<RoomManager>();

            roomManager.Left += Room_Left;
        }

        private void Room_Left(object sender, EventArgs e)
        {
            _floorItems.Clear();
            _wallItems.Clear();
        }

        private void SetHidden(ItemType type, int id, bool hide)
        {
            Furni furni;

            if (type == ItemType.Floor)
            {
                if (!_floorItems.TryGetValue(id, out FloorItem? item))
                    return;

                if (item.IsHidden == hide) return;

                item.IsHidden = hide;
                furni = _floorItems.AddOrUpdate(
                    id,
                    item,
                    (key, existing) =>
                    {
                        existing.IsHidden = hide;
                        return existing;
                    }
                );
            }
            else if (type == ItemType.Wall)
            {
                if (!_wallItems.TryGetValue(id, out WallItem item))
                    return;

                if (item.IsHidden == hide) return;

                item.IsHidden = hide;
                furni = _wallItems.AddOrUpdate(
                    id,
                    item,
                    (key, existing) =>
                    {
                        existing.IsHidden = hide;
                        return existing;
                    }
                );
            }
            else
            {
                return;
            }

            if (hide)
            {
                if (furni.Type == ItemType.Floor)
                {
                    // RemoveFloorItem "id" isExpired pickerId delay
                    // @Update SendLocalAsync(In.RemoveFloorItem, $"{furni.Id}", false, -1, 0);
                }
                else if (furni.Type == ItemType.Wall)
                {
                    // RemoveWallItem "id" pickerId
                    // @Update SendLocalAsync(In.RemoveWallItem, $"{furni.Id}", -1);
                }
            }
            else
            {
                // @Update SendLocalAsync(furni.Type == ItemType.Floor ? In.AddFloorItem : In.AddWallItem, furni);
            }

            OnFurniVisibilityToggled(furni);
        }

        public void Show(ItemType type, int id) => SetHidden(type, id, false);
        public void Hide(ItemType type, int id) => SetHidden(type, id, true);

        #region - Floor items -
        [Receive(nameof(Incoming.ActiveObjects))]
        protected void HandleRoomFloorItems(IReadOnlyPacket packet)
        {
            if (!roomManager.IsLoadingRoom)
            {
                DebugUtil.Log("not entering room");
                return;
            }

            var newItems = new List<FloorItem>();

            var items = FloorItem.ParseAll(packet);
            foreach (FloorItem item in items)
            {
                if (_floorItems.TryAdd(item.Id, item))
                {
                    newItems.Add(item);
                }
                else
                {
                    DebugUtil.Log($"failed to add item {item.Id} to the dictionary");
                }
            }

            if (newItems.Count > 0)
            {
                DebugUtil.Log($"loaded {newItems.Count} items");
                OnFloorItemsLoaded(newItems);
            }
        }

        // @Update [Group(Features.FloorItemManagement), Receive("AddFloorItem")]
        protected void HandleAddFloorItem(IReadOnlyPacket packet)
        {
            if (!roomManager.IsInRoom)
                return;

            var item = FloorItem.Parse(packet);

            if (_floorItems.TryAdd(item.Id, item))
            {
                OnFloorItemAdded(item);
            }
            else
            {
                DebugUtil.Log($"failed to add item {item.Id} to the dictionary");
            }
        }

        // @Update [Group(Features.FloorItemManagement), Receive("RemoveFloorItem")]
        protected void HandleRemoveFloorItem(IReadOnlyPacket packet)
        {
            /*
                string id
                bool isExpired
                int pickerId
                int delay
            */

            if (!roomManager.IsInRoom)
                return;

            string idString = packet.ReadString();
            if (!int.TryParse(idString, out int id)) return;

            if (_floorItems.TryRemove(id, out FloorItem item))
            {
                OnFloorItemRemoved(item);
            }
            else
            {
                DebugUtil.Log($"failed to remove item {id} from the dictionary");
            }
        }

        // @Update [Group(Features.FloorItemUpdates), Receive("FloorItemUpdate")]
        protected void HandleFloorItemUpdate(IReadOnlyPacket packet)
        {
            if (!roomManager.IsInRoom)
                return;

            var updatedItem = FloorItem.Parse(packet);

            if (_floorItems.TryGetValue(updatedItem.Id, out FloorItem previousItem))
            {
                updatedItem.OwnerName = previousItem.OwnerName;
                updatedItem.IsHidden = previousItem.IsHidden;

                if (_floorItems.TryUpdate(updatedItem.Id, updatedItem, previousItem))
                {
                    OnFloorItemUpdated(previousItem, updatedItem);
                }
                else
                {
                    DebugUtil.Log($"failed to update item {updatedItem.Id}");
                }
            }
            else
            {
                DebugUtil.Log($"unable to find item {updatedItem.Id} to update");
            }
        }

        // @Update [Group(Features.FloorItemUpdates), Receive("ObjectOnRoller")]
        protected void HandleObjectOnRoller(IReadOnlyPacket packet)
        {
            if (!roomManager.IsInRoom)
                return;

            var rollerUpdate = RollerUpdate.Parse(packet);
            foreach (var objectUpdate in rollerUpdate.ObjectUpdates)
            {
                if (_floorItems.TryGetValue(objectUpdate.Id, out FloorItem item))
                {
                    var previousTile = item.Location;
                    item.Location = new Tile(rollerUpdate.TargetX, rollerUpdate.TargetY, objectUpdate.TargetZ);
                    OnFloorItemSlide(item, previousTile, rollerUpdate.RollerId);
                }
                else
                {
                    DebugUtil.Log($"unable to find floor item {objectUpdate.Id} to update");
                }
            }
        }

        // @Update [Group(Features.FloorItemUpdates), Receive("ItemExtraData")]
        protected void HandleItemExtraData(IReadOnlyPacket packet)
        {
            if (!roomManager.IsInRoom)
                return;

            string idString = packet.ReadString();
            if (!int.TryParse(idString, out int id)) return;

            if (!_floorItems.TryGetValue(id, out FloorItem item))
            {
                DebugUtil.Log($"unable to find floor item {id} to update");
                return;
            }

            var previousData = item.Data;
            item.Data = ItemData.Parse(packet);

            OnFloorItemDataUpdated(item, previousData);
        }

        // @Update [Group(Features.FloorItemUpdates), Receive("ItemsDataUpdate")]
        protected void HandleItemsDataUpdate(IReadOnlyPacket packet)
        {
            if (!roomManager.IsInRoom)
                return;

            int n = packet.ReadInt();
            for (int i = 0; i < n; i++)
            {
                int itemId = packet.ReadInt();
                var data = ItemData.Parse(packet);
                if (!_floorItems.TryGetValue(itemId, out FloorItem item)) continue;

                var previousData = item.Data;
                item.Data = data;

                OnFloorItemDataUpdated(item, previousData);
            }
        }
        #endregion

        #region - Wall Items -
        // @Update [Group(Features.WallItemManagement), Receive("RoomWallItems")]
        protected void HandleWallItems(IReadOnlyPacket packet)
        {
            if (!roomManager.IsLoadingRoom)
            {
                DebugUtil.Log("not entering room");
                return;
            }

            var newItems = new List<WallItem>();

            var items = WallItem.ParseAll(packet);
            foreach (var item in items)
            {
                if (_wallItems.TryAdd(item.Id, item))
                {
                    newItems.Add(item);
                }
                else
                {
                    DebugUtil.Log($"failed to add wall item with ID {item.Id}");
                }
            }

            if (newItems.Count > 0)
            {
                DebugUtil.Log($"loaded {newItems.Count} items");
                OnWallItemsLoaded(newItems);
            }
        }

        // @Update [Group(Features.WallItemManagement), Receive("AddWallItem")]
        protected void HandleAddWallItem(IReadOnlyPacket packet)
        {
            if (!roomManager.IsInRoom)
                return;
            

            var item = WallItem.Parse(packet);
            if (_wallItems.TryAdd(item.Id, item))
            {
                OnWallItemAdded(item);
            }
            else
            {
                DebugUtil.Log($"failed to add item {item.Id} to the dictionary");
            }
        }

        // @Update [Group(Features.WallItemManagement), Receive("RemoveWallItem")]
        protected void HandleRemoveWallItem(IReadOnlyPacket packet)
        {
            if (!roomManager.IsInRoom)
                return;
            

            string idString = packet.ReadString();
            if (!int.TryParse(idString, out int id)) return;

            if (_wallItems.TryRemove(id, out WallItem item))
            {
                OnWallItemRemoved(item);
            }
            else
            {
                DebugUtil.Log($"failed to remove item {id} from the dictionary");
            }
        }

        // @Update [Group(Features.WallItemUpdates), Receive("WallItemUpdate")]
        protected void HandleWallItemUpdate(IReadOnlyPacket packet)
        {
            if (!roomManager.IsInRoom)
                return;
            

            var updatedItem = WallItem.Parse(packet);
            WallItem previousItem = null;

            updatedItem = _wallItems.AddOrUpdate(
                updatedItem.Id,
                updatedItem,
                (id, existing) =>
                {
                    previousItem = existing;
                    updatedItem.OwnerName = previousItem.OwnerName;
                    updatedItem.IsHidden = previousItem.IsHidden;
                    return updatedItem;
                }
            );

            if (previousItem == null)
            {
                DebugUtil.Log($"failed to find previous wall item for update {updatedItem.Id}");
            }
            else
            {
                OnWallItemUpdated(previousItem, updatedItem);
            }

            /*
            if (wallItems.TryGetValue(updatedItem.Id, out WallItem previousItem))
            {
                updatedItem.OwnerName = previousItem.OwnerName;
                updatedItem.IsHidden = previousItem.IsHidden;

                if (wallItems.TryUpdate(updatedItem.Id, updatedItem, previousItem))
                {
                    OnWallItemUpdated(previousItem, updatedItem);
                }
                else
                {
                    DebugUtil.Log($"failed to update wall item {updatedItem.Id}");
                }
            }
            else
            {
                DebugUtil.Log($"failed to find wall item {updatedItem.Id} to update");
            }*/
        }
        #endregion

        #region - Interaction -
        public void Place(long itemId, int x, int y, int direction)
            => throw new NotImplementedException(); // @Update SendAsync(Out.RoomPlaceItem, $"{itemId} {x} {y} {direction}");
        public void Place(long itemId, (int X, int Y) location, int direction)
            => throw new NotImplementedException(); // @Update SendAsync(Out.RoomPlaceItem, $"{itemId} {location.X} {location.Y} {direction}");
        public void Place(IInventoryItem item, int x, int y, int direction)
            => Place(item.Id, x, y, direction);
        public void Place(IInventoryItem item, (int X, int Y) location, int direction)
            => Place(item.Id, location, direction);

        public void Place(long itemId, WallLocation location)
            => throw new NotImplementedException(); // @Update SendAsync(Out.RoomPlaceItem, $"{itemId} {location}");
        public void Place(IInventoryItem item, WallLocation location)
            => Place(item.Id, location);

        public void Move(long floorItemId, int x, int y, int direction)
            => throw new NotImplementedException(); // @Update SendAsync(Out.RotateMoveItem, floorItemId, x, y, direction);
        public void Move(long floorItemId, (int X, int Y) location, int direction)
            => throw new NotImplementedException(); // @Update SendAsync(Out.RotateMoveItem, floorItemId, location.X, location.Y, direction);
        public void Move(IFloorItem item, int x, int y, int direction)
            => Move(item.Id, x, y, direction);
        public void Move(IFloorItem item, (int X, int Y) location, int direction)
            => Move(item.Id, location.X, location.Y, direction);

        public void Move(long wallItemId, WallLocation location)
            => SendAsync(Out.MoveWallItem, wallItemId, location);
        public void Move(IWallItem item, WallLocation location)
            => Move(item.Id, location);

        public void Pickup(IFurni item) => Pickup(item.Type, item.Id);
        public void Pickup(ItemType type, long id)
        {
            if (type == ItemType.Floor)
                throw new NotImplementedException(); // @Update SendAsync(Out.RoomPickupItem, 2, id);
            else if (type == ItemType.Wall)
                throw new NotImplementedException(); // @Update SendAsync(Out.RoomPickupItem, 1, id);
        }

        public void UpdateStackTile(IFloorItem stackTile, float height) => UpdateStackTile(stackTile.Id, height);
        public void UpdateStackTile(long stackTileId, float height)
            => throw new NotImplementedException(); // @Update SendAsync(Out.SetStackHelperHeight, stackTileId, (int)Math.Round(height * 100.0));
        #endregion
    }
}
