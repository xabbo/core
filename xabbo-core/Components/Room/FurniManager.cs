﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xabbo.Core.Components;
using Xabbo.Core.Events;
using Xabbo.Core.Messages;
using Xabbo.Core.Protocol;

namespace Xabbo.Core.Components
{
    [Dependencies(typeof(RoomManager))]
    public class FurniManager : XabboComponent
    {
        public enum Features { ItemUpdates }

        private RoomManager roomManager;

        private readonly ConcurrentDictionary<int, FloorItem> floorItems;
        private readonly ConcurrentDictionary<int, WallItem> wallItems;

        public IEnumerable<FloorItem> FloorItems => floorItems.Select(item => item.Value);
        public IEnumerable<WallItem> WallItems => wallItems.Select(item => item.Value);
        public IEnumerable<Furni> Furni => ((IEnumerable<Furni>)FloorItems).Concat(WallItems);

        #region - Events -
        public event EventHandler<FloorItemsEventArgs> FloorItemsLoaded;
        public event EventHandler<FloorItemEventArgs> FloorItemAdded;
        public event EventHandler<FloorItemUpdatedEventArgs> FloorItemUpdated;
        public event EventHandler<FloorItemDataUpdatedEventArgs> FloorItemDataUpdated;
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

        #region - Floor Items -
        [Receive("RoomFloorItems")]
        private void HandleRoomFloorItems(Packet packet)
        {
            if (!roomManager.IsEnteringRoom) return;

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
                OnFloorItemsLoaded(newItems);
        }

        [Receive("AddFloorItem")]
        private void HandleAddFloorItem(Packet packet)
        {
            if (!roomManager.IsInRoom) return;

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
        private void HandleRemoveFloorItem(Packet packet)
        {
            if (!roomManager.IsInRoom) return;

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
        private void HandleFloorItemUpdate(Packet packet)
        {
            if (!roomManager.IsInRoom) return;

            var updatedItem = FloorItem.Parse(packet);

            if (!floorItems.TryGetValue(updatedItem.Id, out FloorItem previousItem))
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

        [Group(Features.ItemUpdates), Receive("ObjectOnRoller")]
        private void HandleObjectOnRoller(Packet packet)
        {
            if (!roomManager.IsInRoom) return;

            var rollerUpdate = RollerUpdate.Parse(packet);
            foreach (var objectUpdate in rollerUpdate.ObjectUpdates)
            {
                if (floorItems.TryGetValue(objectUpdate.Id, out FloorItem item))
                {
                    item.Tile = new Tile(rollerUpdate.TargetX, rollerUpdate.TargetY, objectUpdate.TargetZ);
                }
                else
                {
                    DebugUtil.Log($"unable to find floor item {objectUpdate.Id} to update");
                }
            }
        }

        [Group(Features.ItemUpdates), Receive("ItemExtraData")]
        private void HandleItemExtraData(Packet packet)
        {
            if (!roomManager.IsInRoom) return;

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

        [Group(Features.ItemUpdates), Receive("ItemsDataUpdate")]
        private void HandleItemsDataUpdate(Packet packet)
        {
            if (!roomManager.IsInRoom) return;

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
        private void HandleWallItems(Packet packet)
        {
            if (!roomManager.IsEnteringRoom) return;

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
                OnWallItemsLoaded(newItems);
        }

        [Receive("AddWallItem")]
        private void HandleAddWallItem(Packet packet)
        {
            if (!roomManager.IsInRoom) return;

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

        [Group(Features.ItemUpdates), Receive("WallItemUpdate")]
        private void HandleWallItemUpdate(Packet packet)
        {
            if (!roomManager.IsInRoom) return;

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
        private void HandleRemoveWallItem(Packet packet)
        {
            if (!roomManager.IsInRoom) return;

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
