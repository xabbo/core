using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Xabbo.Core.Metadata;

namespace Xabbo.Core
{
    public static class XabboCoreExtensions
    {
        #region - Items -
        public static IEnumerable<T> GetFloorItems<T>(this IEnumerable<T> furni) where T : IItem => furni.Where(x => x.IsFloorItem);
        public static IEnumerable<T> GetWallItems<T>(this IEnumerable<T> furni) where T : IItem => furni.Where(x => x.IsWallItem);

        public static IEnumerable<T> OfKind<T>(this IEnumerable<T> items, ItemType type, int kind) where T : IItem
            => items.Where(item => item.Type == type && item.Kind == kind);
        public static IEnumerable<T> OfKind<T>(this IEnumerable<T> items, FurniInfo furniInfo) where T : IItem
            => OfKind(items, furniInfo.Type, furniInfo.Id);

        public static IEnumerable<T> OfAnyKind<T>(this IEnumerable<T> items, params FurniInfo[] infos) where T : IItem
        {
            var hashSet = new HashSet<(ItemType, int)>(infos.Select(info => (info.Type, info.Id)));
            return items.Where(item => hashSet.Contains((item.Type, item.Kind)));
        }
        public static IEnumerable<T> OfAnyKind<T>(this IEnumerable<T> items, IEnumerable<FurniInfo> infos) where T : IItem
            => OfAnyKind(items, infos.ToArray());
        #endregion

        #region - Furni -
        public static IEnumerable<IFloorItem> OfKind(this IEnumerable<IFloorItem> items, int kind)
            => items.Where(item => item.Kind == kind);
        public static IEnumerable<IWallItem> OfKind(this IEnumerable<IWallItem> items, int kind)
            => items.Where(item => item.Kind == kind);

        public static IEnumerable<IFloorItem> OfAnyKind(this IEnumerable<IFloorItem> items, params int[] kinds)
            => items.Where(item => kinds.Contains(item.Kind));
        public static IEnumerable<IFloorItem> OfAnyKind(this IEnumerable<IFloorItem> items, IEnumerable<int> kinds)
            => items.Where(item => kinds.Contains(item.Kind));

        public static IEnumerable<IWallItem> OfAnyKind(this IEnumerable<IWallItem> items, params int[] kinds)
            => items.Where(item => kinds.Contains(item.Kind));
        public static IEnumerable<IWallItem> OfAnyKind(this IEnumerable<IWallItem> items, IEnumerable<int> kinds)
            => items.Where(item => kinds.Contains(item.Kind));

        public static IEnumerable<T> OwnedBy<T>(this IEnumerable<T> items, int ownerId) where T : IFurni
            => items.Where(item => item.OwnerId == ownerId);
        public static IEnumerable<T> OwnedBy<T>(this IEnumerable<T> items, string ownerName) where T : IFurni
            => items.Where(item => string.Equals(item.OwnerName, ownerName, StringComparison.InvariantCultureIgnoreCase));

        public static IEnumerable<IFloorItem> At(this IEnumerable<IFloorItem> items,
            int? x = null, int? y = null, double? z = null, int? dir = null,
            double epsilon = XabboConst.DEFAULT_EPSILON)
        {
            foreach (var item in items)
            {
                if (x.HasValue && item.X != x.Value) continue;
                if (y.HasValue && item.Y != y.Value) continue;
                if (z.HasValue && Math.Abs(item.Z - z.Value) >= epsilon) continue;
                if (dir.HasValue && item.Direction != dir.Value) continue;
                yield return item;
            }
        }

        public static IEnumerable<IFloorItem> At(this IEnumerable<IFloorItem> items,
            (int X, int Y) location, int? dir = null)
        {
            return At(items, location.X, location.Y, dir: dir);
        }

        public static IEnumerable<IFloorItem> At(this IEnumerable<IFloorItem> items,
            (int X, int Y, double Z) location, int? dir = null, double epsilon = XabboConst.DEFAULT_EPSILON)
        {
            return At(items, location.X, location.Y, location.Z, dir, epsilon);
        }

        public static IEnumerable<IFloorItem> At(this IEnumerable<IFloorItem> items,
            Tile tile, int? dir = null, double epsilon = XabboConst.DEFAULT_EPSILON)
        { 
            return At(items, tile.X, tile.Y, tile.Z, dir, epsilon: epsilon);
        }

        public static IEnumerable<IWallItem> At(this IEnumerable<IWallItem> items,
            int? wallX = null, int? wallY = null, int? x = null, int? y = null,
            WallOrientation orientation = null)
        {
            foreach (var item in items)
            {
                if (wallX.HasValue && item.WallX != wallX) continue;
                if (wallY.HasValue && item.WallY != wallY) continue;
                if (x.HasValue && item.X != x) continue;
                if (y.HasValue && item.Y != y) continue;
                if (orientation != null && item.Orientation != orientation) continue;
                yield return item;
            }
        }

        public static IEnumerable<IWallItem> At(this IEnumerable<IWallItem> items, IWallLocation location)
            => At(items, location.WallX, location.WallY, location.X, location.Y, location.Orientation);
        #endregion

        #region - Inventory -
        public static IEnumerable<T> OfCategory<T>(this IEnumerable<T> items, FurniCategory category) where T : IInventoryItem
            => items.Where(item => item.Category == category);
        public static IEnumerable<T> GetGroupable<T>(this IEnumerable<T> items) where T : IInventoryItem
            => items.Where(item => item.IsGroupable);
        public static IEnumerable<IInventoryItem> GetTradeable(this IEnumerable<IInventoryItem> items)
            => items.Where(item => item.IsTradeable);
        public static IEnumerable<IInventoryItem> GetSellable(this IEnumerable<IInventoryItem> items)
            => items.Where(item => item.IsSellable);

        public static IEnumerable<IGrouping<int, IInventoryItem>> Group(this IEnumerable<IInventoryItem> items, int maxSlots = 9, int maxItems = 1500)
        {
            if (maxSlots < 1 || maxSlots > 9) throw new ArgumentOutOfRangeException("maxSlots");
            if (maxItems < 1 || maxItems > 1500) throw new ArgumentOutOfRangeException("maxItems");

            int groupIndex = 0, currentSlots = 0, currentCount = 0;
            var lastKind = ((ItemType)(-1), -1);

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
        #endregion

        #region - Rooms -
        public static IEnumerable<IRoomInfo> Filter(
            this IEnumerable<IRoomInfo> rooms,
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
        #endregion

        #region - Entities -
        public static IEnumerable<T> At<T>(this IEnumerable<T> entities,
            int? x = null, int? y = null, double? z = null, int? dir = null,
            double epsilon = XabboConst.DEFAULT_EPSILON) where T : IEntity
        {
            foreach (var e in entities)
            {
                if (e.Location == null) continue;
                if (x.HasValue && e.Location.X != x) continue;
                if (y.HasValue && e.Location.Y != y) continue;
                if (z.HasValue && Math.Abs(e.Location.Z - z.Value) >= epsilon) continue;
                if (dir.HasValue && e.Direction != dir.Value) continue;
                yield return e;
            }
        }

        public static IEnumerable<T> At<T>(this IEnumerable<T> entities,
            (int X, int Y) location, int? dir = null) where T : IEntity
        {
            return At(entities, location.X, location.Y, null, dir);
        }

        public static IEnumerable<T> At<T>(this IEnumerable<T> entities,
            (int X, int Y, double Z) location, int? dir = null,
            double epsilon = XabboConst.DEFAULT_EPSILON) where T : IEntity
        { 
            return At(entities, location.X, location.Y, location.Z, dir, epsilon);
        }

        public static IEnumerable<T> Inside<T>(this IEnumerable<T> entities, Area area)
            where T : IEntity
        {
            foreach (var e in entities)
            {
                if (e.Location == null) continue;
                if (area.Contains(e.Location))
                    yield return e;
            }
        }

        public static IEnumerable<T> Outside<T>(this IEnumerable<T> entities, Area area)
            where T : IEntity
        {
            foreach (var e in entities)
            {
                if (e.Location == null) continue;
                if (!area.Contains(e.Location))
                    yield return e;
            }
        }
        #endregion
    }
}
