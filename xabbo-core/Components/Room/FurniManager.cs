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

        private readonly ConcurrentDictionary<int, FloorItem> floorItems;
        private readonly ConcurrentDictionary<int, WallItem> wallItems;

        public IEnumerable<IFloorItem> FloorItems => floorItems.Select(item => item.Value);
        public IEnumerable<IWallItem> WallItems => wallItems.Select(item => item.Value);
        public IEnumerable<IFurni> Furni => FloorItems.Concat<IFurni>(WallItems);

        /// <summary>
        /// Gets whether the floor item with the specified ID exists in the room or not.
        /// </summary>
        public bool FloorItemExists(int itemId) => floorItems.ContainsKey(itemId);
        /// <summary>
        /// Gets whether the wall item with the specified ID exists in the room or not.
        /// </summary>
        public bool WallItemExists(int itemId) => wallItems.ContainsKey(itemId);

        /// <summary>
        /// Gets the furni of the specified type with the specified ID, or <c>null</c> if it does not exist.
        /// </summary>
        public IFurni GetFurni(ItemType type, int itemId)
        {
            if (type == ItemType.Floor) return GetFloorItem(itemId);
            else if (type == ItemType.Wall) return GetWallItem(itemId);
            else throw new InvalidOperationException($"ItemType must be Floor or Wall");
        }

        /// <summary>
        /// Gets the floor item with the specified ID or <c>null</c> if it does not exist.
        /// </summary>
        public IFloorItem GetFloorItem(int itemId) => floorItems.TryGetValue(itemId, out FloorItem item) ? item : null;
        /// <summary>
        /// Gets the wall item with the specified ID or <c>null</c> if it does not exist.
        /// </summary>
        public IWallItem GetWallItem(int itemId) => wallItems.TryGetValue(itemId, out WallItem item) ? item : null;

        #region - Events -
        /// <summary>
        /// Invoked when the floor items are loaded.
        /// This may happen multiple times depending on
        /// how many items are in the room.
        /// </summary>
        public event EventHandler<FloorItemsEventArgs> FloorItemsLoaded;
        /// <summary>
        /// Invoked when a floor item is added to the room.
        /// </summary>
        public event EventHandler<FloorItemEventArgs> FloorItemAdded;
        /// <summary>
        /// Invoked when a floor item is updated.
        /// </summary>
        public event EventHandler<FloorItemUpdatedEventArgs> FloorItemUpdated;
        /// <summary>
        /// Invoked when a floor item's data is updated.
        /// </summary>
        public event EventHandler<FloorItemDataUpdatedEventArgs> FloorItemDataUpdated;
        /// <summary>
        /// Invoked when a floor item slides due to a roller or wired update.
        /// </summary>
        public event EventHandler<FloorItemSlideEventArgs> FloorItemSlide;
        /// <summary>
        /// Invoked when a floor item is removed from the room.
        /// </summary>
        public event EventHandler<FloorItemEventArgs> FloorItemRemoved;

        /// <summary>
        /// Invoked when the wall items are loaded.
        /// This may happen multiple times depending on
        /// how many items are in the room.
        /// </summary>
        public event EventHandler<WallItemsEventArgs> WallItemsLoaded;
        /// <summary>
        /// Invoked when a wall item is added to the room.
        /// </summary>
        public event EventHandler<WallItemEventArgs> WallItemAdded;
        /// <summary>
        /// Invoked when a wall item is udpated.
        /// </summary>
        public event EventHandler<WallItemUpdatedEventArgs> WallItemUpdated;
        /// <summary>
        /// Invoked when a wall item is removed from the room.
        /// </summary>
        public event EventHandler<WallItemEventArgs> WallItemRemoved;

        protected virtual void OnFloorItemsLoaded(IEnumerable<FloorItem> items)
            => FloorItemsLoaded?.Invoke(this, new FloorItemsEventArgs(items));
        protected virtual void OnFloorItemAdded(FloorItem item)
            => FloorItemAdded?.Invoke(this, new FloorItemEventArgs(item));
        protected virtual void OnFloorItemUpdated(FloorItem previousItem, FloorItem updatedItem)
            => FloorItemUpdated?.Invoke(this, new FloorItemUpdatedEventArgs(previousItem, updatedItem));
        protected virtual void OnFloorItemDataUpdated(FloorItem item, ItemData previousData)
            => FloorItemDataUpdated?.Invoke(this, new FloorItemDataUpdatedEventArgs(item, previousData));
        protected virtual void OnFloorItemSlide(FloorItem item, Tile previousTile, int rollerId)
            => FloorItemSlide?.Invoke(this, new FloorItemSlideEventArgs(item, previousTile, rollerId));
        protected virtual void OnFloorItemRemoved(FloorItem item)
            => FloorItemRemoved?.Invoke(this, new FloorItemEventArgs(item));

        protected virtual void OnWallItemsLoaded(IEnumerable<WallItem> items)
            => WallItemsLoaded?.Invoke(this, new WallItemsEventArgs(items));
        protected virtual void OnWallItemAdded(WallItem item)
            => WallItemAdded?.Invoke(this, new WallItemEventArgs(item));
        protected virtual void OnWallItemUpdated(WallItem previousItem, WallItem updatedItem)
            => WallItemUpdated?.Invoke(this, new WallItemUpdatedEventArgs(previousItem, updatedItem));
        protected virtual void OnWallItemRemoved(WallItem item)
            => WallItemRemoved?.Invoke(this, new WallItemEventArgs(item));
        #endregion

        public FurniManager()
        {
            floorItems = new ConcurrentDictionary<int, FloorItem>();
            wallItems = new ConcurrentDictionary<int, WallItem>();
        }

        protected override void OnInitialize()
        {
            roomManager = GetComponent<RoomManager>();

            roomManager.Left += Room_Left;
        }

        private void Room_Left(object sender, EventArgs e)
        {
            floorItems.Clear();
            wallItems.Clear();
        }

        public void SetHidden(ItemType type, int id, bool hide)
        {
            Furni furni;

            if (type == ItemType.Floor)
            {
                if (!floorItems.TryGetValue(id, out FloorItem item))
                    return;

                if (item.IsHidden == hide) return;

                item.IsHidden = hide;
                furni = floorItems.AddOrUpdate(
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
                if (!wallItems.TryGetValue(id, out WallItem item))
                    return;

                item.IsHidden = hide;
                furni = wallItems.AddOrUpdate(
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
                    SendLocalAsync(In.RemoveFloorItem, furni.Id.ToString(), false, -1, 0);
                }
                else if (furni.Type == ItemType.Wall)
                {
                    // RemoveWallItem "id" pickerId
                    SendLocalAsync(In.RemoveWallItem, furni.Id.ToString(), -1);
                }
            }
            else
            {
                SendLocalAsync(furni.Type == ItemType.Floor ? In.AddFloorItem : In.AddWallItem, furni);
            }
        }

        public void Hide(IFurni furni) => SetHidden(furni.Type, furni.Id, true);

        public void Show(IFurni furni) => SetHidden(furni.Type, furni.Id, false);

        #region - Floor items -
        [Group(Features.FloorItemManagement), Receive("RoomFloorItems")]
        protected void HandleRoomFloorItems(IReadOnlyPacket packet)
        {
            if (!roomManager.IsLoadingRoom)
            {
                DebugUtil.Log("not entering room");
                return;
            }

            var newItems = new List<FloorItem>();

            var items = FloorItem.ParseAll(packet);
            foreach (var item in items)
            {
                if (floorItems.TryAdd(item.Id, item))
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

        [Group(Features.FloorItemManagement), Receive("AddFloorItem")]
        protected void HandleAddFloorItem(IReadOnlyPacket packet)
        {
            if (!roomManager.IsInRoom)
                return;

            var item = FloorItem.Parse(packet);

            if (floorItems.TryAdd(item.Id, item))
            {
                OnFloorItemAdded(item);
            }
            else
            {
                DebugUtil.Log($"failed to add item {item.Id} to the dictionary");
            }
        }

        [Group(Features.FloorItemManagement), Receive("RemoveFloorItem")]
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

            if (floorItems.TryRemove(id, out FloorItem item))
            {
                OnFloorItemRemoved(item);
            }
            else
            {
                DebugUtil.Log($"failed to remove item {id} from the dictionary");
            }
        }

        [Group(Features.FloorItemUpdates), Receive("FloorItemUpdate")]
        protected void HandleFloorItemUpdate(IReadOnlyPacket packet)
        {
            if (!roomManager.IsInRoom)
                return;

            var updatedItem = FloorItem.Parse(packet);

            if (floorItems.TryGetValue(updatedItem.Id, out FloorItem previousItem))
            {
                updatedItem.OwnerName = previousItem.OwnerName;
                updatedItem.IsHidden = previousItem.IsHidden;

                if (floorItems.TryUpdate(updatedItem.Id, updatedItem, previousItem))
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

        [Group(Features.FloorItemUpdates), Receive("ObjectOnRoller")]
        protected void HandleObjectOnRoller(IReadOnlyPacket packet)
        {
            if (!roomManager.IsInRoom)
                return;

            var rollerUpdate = RollerUpdate.Parse(packet);
            foreach (var objectUpdate in rollerUpdate.ObjectUpdates)
            {
                if (floorItems.TryGetValue(objectUpdate.Id, out FloorItem item))
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

        [Group(Features.FloorItemUpdates), Receive("ItemExtraData")]
        protected void HandleItemExtraData(IReadOnlyPacket packet)
        {
            if (!roomManager.IsInRoom)
                return;

            string idString = packet.ReadString();
            if (!int.TryParse(idString, out int id)) return;

            if (!floorItems.TryGetValue(id, out FloorItem item))
            {
                DebugUtil.Log($"unable to find floor item {id} to update");
                return;
            }

            var previousData = item.Data;
            item.Data = ItemData.Parse(packet);

            OnFloorItemDataUpdated(item, previousData);
        }

        [Group(Features.FloorItemUpdates), Receive("ItemsDataUpdate")]
        protected void HandleItemsDataUpdate(IReadOnlyPacket packet)
        {
            if (!roomManager.IsInRoom)
                return;

            int n = packet.ReadInt();
            for (int i = 0; i < n; i++)
            {
                int itemId = packet.ReadInt();
                var data = ItemData.Parse(packet);
                if (!floorItems.TryGetValue(itemId, out FloorItem item)) continue;

                var previousData = item.Data;
                item.Data = data;

                OnFloorItemDataUpdated(item, previousData);
            }
        }
        #endregion

        #region - Wall Items -
        [Group(Features.WallItemManagement), Receive("RoomWallItems")]
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
                if (wallItems.TryAdd(item.Id, item))
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

        [Group(Features.WallItemManagement), Receive("AddWallItem")]
        protected void HandleAddWallItem(IReadOnlyPacket packet)
        {
            if (!roomManager.IsInRoom)
                return;
            

            var item = WallItem.Parse(packet);
            if (wallItems.TryAdd(item.Id, item))
            {
                OnWallItemAdded(item);
            }
            else
            {
                DebugUtil.Log($"failed to add item {item.Id} to the dictionary");
            }
        }

        [Group(Features.WallItemManagement), Receive("RemoveWallItem")]
        protected void HandleRemoveWallItem(IReadOnlyPacket packet)
        {
            if (!roomManager.IsInRoom)
                return;
            

            string idString = packet.ReadString();
            if (!int.TryParse(idString, out int id)) return;

            if (wallItems.TryRemove(id, out WallItem item))
            {
                OnWallItemRemoved(item);
            }
            else
            {
                DebugUtil.Log($"failed to remove item {id} from the dictionary");
            }
        }

        [Group(Features.WallItemUpdates), Receive("WallItemUpdate")]
        protected void HandleWallItemUpdate(IReadOnlyPacket packet)
        {
            if (!roomManager.IsInRoom)
                return;
            

            var updatedItem = WallItem.Parse(packet);
            WallItem previousItem = null;

            updatedItem = wallItems.AddOrUpdate(
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
        public void Place(int itemId, int x, int y, int direction)
            => SendAsync(Out.RoomPlaceItem, $"{itemId} {x} {y} {direction}");
        public void Place(int itemId, (int X, int Y) location, int direction)
            => SendAsync(Out.RoomPlaceItem, $"{itemId} {location.X} {location.Y} {direction}");
        public void Place(IInventoryItem item, int x, int y, int direction)
            => Place(item.Id, x, y, direction);
        public void Place(IInventoryItem item, (int X, int Y) location, int direction)
            => Place(item.Id, location, direction);

        public void Place(int itemId, WallLocation location)
            => SendAsync(Out.RoomPlaceItem, $"{itemId} {location}");
        public void Place(IInventoryItem item, WallLocation location)
            => Place(item.Id, location);

        public void Move(int floorItemId, int x, int y, int direction)
            => SendAsync(Out.RotateMoveItem, floorItemId, x, y, direction);
        public void Move(int floorItemId, (int X, int Y) location, int direction)
            => SendAsync(Out.RotateMoveItem, floorItemId, location.X, location.Y, direction);
        public void Move(IFloorItem item, int x, int y, int direction)
            => Move(item.Id, x, y, direction);
        public void Move(IFloorItem item, (int X, int Y) location, int direction)
            => Move(item.Id, location.X, location.Y, direction);

        public void Move(int wallItemId, WallLocation location)
            => SendAsync(Out.MoveWallItem, wallItemId, location);
        public void Move(IWallItem item, WallLocation location)
            => Move(item.Id, location);

        public void Pickup(IFurni item) => Pickup(item.Type, item.Id);
        public void Pickup(ItemType type, int id)
        {
            if (type == ItemType.Floor)
                SendAsync(Out.RoomPickupItem, 2, id);
            else if (type == ItemType.Wall)
                SendAsync(Out.RoomPickupItem, 1, id);
        }

        public void UpdateStackTile(IFloorItem stackTile, double height) => UpdateStackTile(stackTile.Id, height);
        public void UpdateStackTile(int stackTileId, double height)
            => SendAsync(Out.SetStackHelperHeight, stackTileId, (int)Math.Round(height * 100.0));
        #endregion
    }
}
