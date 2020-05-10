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
        public enum Features { Tracking }

        private RoomManager roomManager;

        private readonly ConcurrentDictionary<int, FloorItem> floorItems;
        private readonly ConcurrentDictionary<int, WallItem> wallItems;

        public IEnumerable<FloorItem> FloorItems => floorItems.Select(item => item.Value);
        public IEnumerable<WallItem> WallItems => wallItems.Select(item => item.Value);
        public IEnumerable<Furni> Furni => ((IEnumerable<Furni>)FloorItems).Concat(WallItems);

        public bool FloorItemExists(int itemId) => FloorItems.Any(item => item.Id == itemId);
        public bool WallItemExists(int itemId) => WallItems.Any(item => item.Id == itemId);

        public FloorItem GetFloorItem(int itemId) => FloorItems.FirstOrDefault(item => item.Id == itemId);
        public WallItem GetWallItem(int itemId) => WallItems.FirstOrDefault(item => item.Id == itemId);

        #region - Events -
        public event EventHandler<FloorItemsEventArgs> FloorItemsLoaded;
        public event EventHandler<FloorItemEventArgs> FloorItemAdded;
        public event EventHandler<FloorItemUpdatedEventArgs> FloorItemUpdated;
        public event EventHandler<FloorItemDataUpdatedEventArgs> FloorItemDataUpdated;
        public event EventHandler<FloorItemSlideEventArgs> FloorItemSlide;
        public event EventHandler<FloorItemEventArgs> FloorItemRemoved;

        public event EventHandler<WallItemsEventArgs> WallItemsLoaded;
        public event EventHandler<WallItemEventArgs> WallItemAdded;
        public event EventHandler<WallItemUpdatedEventArgs> WallItemUpdated;
        public event EventHandler<WallItemEventArgs> WallItemRemoved;

        protected virtual void OnFloorItemsLoaded(IEnumerable<FloorItem> items)
            => FloorItemsLoaded?.Invoke(this, new FloorItemsEventArgs(items));
        protected virtual void OnFloorItemAdded(FloorItem item)
            => FloorItemAdded?.Invoke(this, new FloorItemEventArgs(item));
        protected virtual void OnFloorItemUpdated(FloorItem previousItem, FloorItem updatedItem)
            => FloorItemUpdated?.Invoke(this, new FloorItemUpdatedEventArgs(previousItem, updatedItem));
        protected virtual void OnFloorItemDataUpdated(FloorItem item, StuffData previousData)
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
            DebugUtil.Log("clearing items");

            floorItems.Clear();
            wallItems.Clear();
        }

        #region - Floor Items -
        [Receive("RoomFloorItems")]
        protected void HandleRoomFloorItems(Packet packet)
        {
            if (!roomManager.IsEnteringRoom)
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

        [Receive("AddFloorItem")]
        protected void HandleAddFloorItem(Packet packet)
        {
            if (!roomManager.IsInRoom)
            {
                DebugUtil.Log("not in room");
                return;
            }

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

        [Receive("RemoveFloorItem")]
        protected void HandleRemoveFloorItem(Packet packet)
        {
            if (!roomManager.IsInRoom)
            {
                DebugUtil.Log("not in room");
                return;
            }

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

        [Receive("FloorItemUpdate")]
        protected void HandleFloorItemUpdate(Packet packet)
        {
            if (!roomManager.IsInRoom)
            {
                DebugUtil.Log("not in room");
                return;
            }

            var updatedItem = FloorItem.Parse(packet);

            if (floorItems.TryGetValue(updatedItem.Id, out FloorItem previousItem))
            {
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

        [Group(Features.Tracking), Receive("ObjectOnRoller")]
        protected void HandleObjectOnRoller(Packet packet)
        {
            if (!roomManager.IsInRoom)
            {
                DebugUtil.Log("not in room");
                return;
            }

            var rollerUpdate = RollerUpdate.Parse(packet);
            foreach (var objectUpdate in rollerUpdate.ObjectUpdates)
            {
                if (floorItems.TryGetValue(objectUpdate.Id, out FloorItem item))
                {
                    var previousTile = item.Tile;
                    item.Tile = new Tile(rollerUpdate.TargetX, rollerUpdate.TargetY, objectUpdate.TargetZ);
                    OnFloorItemSlide(item, previousTile, rollerUpdate.RollerId);
                }
                else
                {
                    DebugUtil.Log($"unable to find floor item {objectUpdate.Id} to update");
                }
            }
        }

        [Group(Features.Tracking), Receive("ItemExtraData")]
        protected void HandleItemExtraData(Packet packet)
        {
            if (!roomManager.IsInRoom)
            {
                DebugUtil.Log("not in room");
                return;
            }

            string idString = packet.ReadString();
            if (!int.TryParse(idString, out int id)) return;

            if (!floorItems.TryGetValue(id, out FloorItem item))
            {
                DebugUtil.Log($"unable to find floor item {id} to update");
                return;
            }

            var previousData = item.Data;
            item.Data = StuffData.Parse(packet);

            OnFloorItemDataUpdated(item, previousData);
        }

        [Group(Features.Tracking), Receive("ItemsDataUpdate")]
        protected void HandleItemsDataUpdate(Packet packet)
        {
            if (!roomManager.IsInRoom)
            {
                DebugUtil.Log("not in room");
                return;
            }

            int n = packet.ReadInteger();
            for (int i = 0; i < n; i++)
            {
                int itemId = packet.ReadInteger();
                var data = StuffData.Parse(packet);
                if (!floorItems.TryGetValue(itemId, out FloorItem item)) continue;

                var previousData = item.Data;
                item.Data = data;

                OnFloorItemDataUpdated(item, previousData);
            }
        }
        #endregion

        #region - Wall Items -
        [Receive("RoomWallItems")]
        protected void HandleWallItems(Packet packet)
        {
            if (!roomManager.IsEnteringRoom)
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

        [Receive("AddWallItem")]
        protected void HandleAddWallItem(Packet packet)
        {
            if (!roomManager.IsInRoom)
            {
                DebugUtil.Log("not in room");
                return;
            }

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

        [Group(Features.Tracking), Receive("WallItemUpdate")]
        protected void HandleWallItemUpdate(Packet packet)
        {
            if (!roomManager.IsInRoom)
            {
                DebugUtil.Log("not in room");
                return;
            }

            var updatedItem = WallItem.Parse(packet);
            if (wallItems.TryGetValue(updatedItem.Id, out WallItem previousItem))
            {
                updatedItem.OwnerName = previousItem.OwnerName;
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
            }
        }

        [Receive("RemoveWallItem")]
        protected void HandleRemoveWallItem(Packet packet)
        {
            if (!roomManager.IsInRoom)
            {
                DebugUtil.Log("not in room");
                return;
            }

            string idString = packet.ReadString();
            if (!int.TryParse(idString, out int id)) return;

            if (wallItems.TryRemove(id, out WallItem item))
            {
                OnWallItemRemoved(item);
            }
            else
            {
                DebugUtil.Log($"failed to remove item {item.Id} from the dictionary");
            }
        }
        #endregion
    }
}
