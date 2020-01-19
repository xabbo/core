using System;
using System.Collections.Generic;
using System.Linq;

using Xabbo.Core.Metadata;

namespace Xabbo.Core
{
    public static class XabboExtensions
    {
        #region - Item -
        public static IEnumerable<FloorItem> GetFloorItems<T>(this IEnumerable<T> furni) where T : IItem => furni.OfType<FloorItem>();
        public static IEnumerable<WallItem> GetWallItems<T>(this IEnumerable<T> furni) where T : IItem => furni.OfType<WallItem>();
        public static IEnumerable<T> OfKind<T>(this IEnumerable<T> items, FurniInfo furniInfo) where T : IItem
            => items.OfKind(furniInfo.Type, furniInfo.Id);
        public static IEnumerable<T> OfKind<T>(this IEnumerable<T> items, FurniType type, int kind) where T : IItem
            => items.Where(item => item.Type == type && item.Kind == kind);
        #endregion

        #region - Furni -
        public static IEnumerable<FloorItem> OfKind(this IEnumerable<FloorItem> items, int kind)
            => items.Where(item => item.Kind == kind);
        public static IEnumerable<WallItem> OfKind(this IEnumerable<WallItem> items, int kind)
            => items.Where(item => item.Kind == kind);
        public static IEnumerable<FloorItem> At(this IEnumerable<FloorItem> items,
            int? x = null, int? y = null, double? z = null, int? dir = null, double epsilon = 0.001)
        {
            foreach (var item in items)
            {
                if (x.HasValue && item.X != x.Value) continue;
                if (y.HasValue && item.Y != y.Value) continue;
                if (z.HasValue && Math.Abs(item.Z - z.Value) < epsilon) continue;
                if (dir.HasValue && item.Direction != dir.Value) continue;
                yield return item;
            }
        }
        public static bool AnyAt(this IEnumerable<FloorItem> items, int? x = null, int? y = null, double? z = null, int? dir = null)
            => items.At(x, y, z, dir).Any();
        public static IEnumerable<WallItem> At(this IEnumerable<WallItem> items,
            int? wallX = null, int? wallY = null, int? x = null, int? y = null, WallOrientation? orientation = null)
        {
            foreach (var item in items)
            {
                if (wallX.HasValue && item.WallX != wallX) continue;
                if (wallY.HasValue && item.WallY != wallY) continue;
                if (x.HasValue && item.X != x) continue;
                if (y.HasValue && item.Y != y) continue;
                if (orientation.HasValue && item.Orientation != orientation) continue;
                yield return item;
            }
        }
        public static bool AnyAt(this IEnumerable<WallItem> items, int? wallX = null, int? wallY = null, int? x = null, int? y = null, WallOrientation? orientation = null)
            => items.At(wallX, wallY, x, y, orientation).Any();
        #endregion

        #region - Inventory -
        public static IEnumerable<InventoryItem> GetGroupable(this IEnumerable<InventoryItem> items)
            => items.Where(item => item.IsGroupable);
        public static IEnumerable<InventoryItem> GetTradeable(this IEnumerable<InventoryItem> items)
            => items.Where(item => item.IsTradeable);
        public static IEnumerable<InventoryItem> GetSellable(this IEnumerable<InventoryItem> items)
            => items.Where(item => item.IsSellable);

        public static IEnumerable<IGrouping<int, InventoryItem>> Split(this IEnumerable<InventoryItem> items, int maxSlots = 9, int maxItems = 1500)
        {
            if (maxSlots < 1 || maxSlots > 9) throw new ArgumentOutOfRangeException("maxSlots");
            if (maxItems < 1 || maxItems > 1500) throw new ArgumentOutOfRangeException("maxItems");

            int groupIndex = 0, currentSlots = 0, currentCount = 0;
            var lastKind = ((FurniType)(-1), -1);

            return items
                .OrderBy(item => item.Type)
                .ThenBy(item => item.Kind)
                .GroupBy(item =>
                {
                    var kind = (item.Type, item.Kind);
                    if (!item.IsGroupable || lastKind != kind)
                        currentSlots++;
                    currentCount++;
                    lastKind = kind;

                    if (currentSlots > maxSlots || currentCount == maxItems)
                    {
                        currentCount = 0;
                        currentSlots = 1;
                        return ++groupIndex;
                    }

                    return groupIndex;
                });
        }

        /*public static IEnumerable<InventoryItem[]> Partition(this IEnumerable<InventoryItem> items, int maxSlots = 9, int maxItems = 1500)
        {
            int currentSlots = 0;
            var currentItems = new List<InventoryItem>();

            var e = items
                .OrderBy(item => item.Type)
                .ThenBy(item => item.Kind)
                .GetEnumerator();

            while (e.MoveNext())
            {
                if (!e.Current.IsGroupable || !currentItems.Any(it => it.Kind == e.Current.Kind && it.Type == e.Current.Type))
                {
                    currentSlots++;
                }

                if (currentSlots > maxSlots || currentItems.Count == maxItems)
                {
                    yield return currentItems.ToArray();
                    currentItems.Clear();
                    currentSlots = 1;
                }

                currentItems.Add(e.Current);
            }

            if (currentItems.Count > 0)
                yield return currentItems.ToArray();
        }*/
        #endregion
        
        public static IEnumerable<RoomInfo> Find(
            this IEnumerable<RoomInfo> rooms,
            string name = null,
            string description = null,
            int? ownerId = null,
            string owner = null,
            RoomAccess? access = null,
            TradePermissions? trading = null,
            RoomCategory? category = null,
            int? groupId = null,
            string group = null)
        {
            foreach (var roomInfo in rooms)
            {
                if (name != null && !roomInfo.Name.ToLower().Contains(name.ToLower())) continue;
                if (description != null && !roomInfo.Description.ToLower().Contains(description.ToLower())) continue;
                if (ownerId.HasValue && roomInfo.OwnerId != ownerId) continue;
                if (owner != null && !roomInfo.OwnerName.Equals(owner, StringComparison.InvariantCultureIgnoreCase)) continue;
                if (access.HasValue && roomInfo.Access != access) continue;
                if (trading.HasValue && roomInfo.Trading != trading) continue;
                if (category.HasValue && roomInfo.Category != category) continue;
                if (groupId.HasValue && (!roomInfo.IsGroupHomeRoom || roomInfo.GroupId != groupId)) continue;
                if (group != null && (!roomInfo.IsGroupHomeRoom || !roomInfo.GroupName.ToLower().Contains(group.ToLower()))) continue;
                yield return roomInfo;
            }
        }
    }
}
