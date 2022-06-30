﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;

using Xabbo.Core.GameData;

namespace Xabbo.Core.Extensions
{
    /// <summary>
    /// Provides convenient extension methods utilizing Xabbo.Core.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class XabboCoreExtensions
    {
        private static FurniData? _furniData;
        private static ExternalTexts? _texts;

        private static FurniData FurniData => _furniData ?? throw new InvalidOperationException($"{nameof(XabboCoreExtensions)} has not been initialized.");
        private static ExternalTexts Texts => _texts ?? throw new InvalidOperationException($"{nameof(XabboCoreExtensions)} has not been initialized.");
        
        /// <summary>
        /// Initializes <see cref="XabboCoreExtensions"/> with the specified game data.
        /// </summary>
        public static void Initialize(FurniData furniData, ExternalTexts texts)
        {
            _furniData = furniData;
            _texts = texts;
        }

        #region - Items -
        private static string GetVariant(IItem item)
        {
            if (item is IFloorItem floorItem)
            {
                return floorItem.Data.Value;
            }
            else if (item is IWallItem wallItem)
            {
                return wallItem.Data;
            }
            else if (item is IInventoryItem inventoryItem)
            {
                return inventoryItem.Data.Value;
            }
            else if (item is IMarketplaceOffer marketplaceItem)
            {
                return marketplaceItem.Data.Value;
            }
            else if (item is ICatalogProduct catalogProduct)
            {
                return catalogProduct.Variant;
            }
            else
            {
                throw new Exception($"Failed to find variant for item type: {item.GetType()}.");
            }
        }

        /// <summary>
        /// Gets the furni info of this item.
        /// </summary>
        public static FurniInfo GetInfo(this IItem item) => FurniData.GetInfo(item);

        /// <summary>
        /// Gets the identifier of this item.
        /// </summary>
        public static string GetIdentifier(this IItem item) => GetInfo(item).Identifier;

        /// <summary>
        /// Gets the descriptor of this item.
        /// </summary>
        public static ItemDescriptor GetDescriptor(this IItem item)
        {
            FurniInfo info = GetInfo(item);

            string? variant = null;

            if (info.Identifier == "poster")
            {
                variant = GetVariant(item);
            }

            return new ItemDescriptor(item.Type, item.Kind, variant);
        }

        /// <summary>
        /// Gets the name of an item by its descriptor.
        /// </summary>
        public static string GetName(this ItemDescriptor descriptor)
        {
            FurniInfo info = FurniData.GetInfo(descriptor.Type, descriptor.Kind)
                ?? throw new Exception($"Failed to find furni info for {descriptor.Type.ToString().ToLower()} item {descriptor.Kind}.");

            string name = info.Name;

            if (info.Identifier == "poster")
            {
                string key = $"poster_{descriptor.Variant}_name";
                if (Texts.ContainsKey(key))
                {
                    name = Texts[key];
                }
            }

            return string.IsNullOrWhiteSpace(name) ? info.Identifier : name;
        }

        /// <summary>
        /// Gets the name of this item.
        /// </summary>
        public static string GetName(this IItem item) => GetName(GetDescriptor(item));

        /// <summary>
        /// Gets the category of this item.
        /// </summary>
        public static FurniCategory GetCategory(this IItem item) => GetInfo(item)?.Category ?? FurniCategory.Unknown;

        /// <summary>
        /// Gets the category name of this item.
        /// </summary>
        public static string GetCategoryName(this IItem item) => GetInfo(item).CategoryName;

        /// <summary>
        /// Gets the furni line of this item.
        /// </summary>
        public static string GetLine(this IItem item) => GetInfo(item).Line;

        /// <summary>
        /// Selects floor items.
        /// </summary>
        public static IEnumerable<T> GetFloorItems<T>(this IEnumerable<T> items)
            where T : IItem => items.Where(x => x.Type == ItemType.Floor);

        /// <summary>
        /// Selects wall items.
        /// </summary>
        public static IEnumerable<T> GetWallItems<T>(this IEnumerable<T> items)
            where T : IItem => items.Where(x => x.Type == ItemType.Wall);

        /// <summary>
        /// Selects items of the specified type and kind.
        /// </summary>
        public static IEnumerable<T> OfKind<T>(this IEnumerable<T> items, ItemType type, int kind)
            where T : IItem => items.Where(item => item.Type == type && item.Kind == kind);

        /// <summary>
        /// Selects items of the same kind as the specified furni info.
        /// </summary>
        public static IEnumerable<T> OfKind<T>(this IEnumerable<T> items, FurniInfo furniInfo)
            where T : IItem => OfKind(items, furniInfo.Type, furniInfo.Kind);

        /// <summary>
        /// Selects items of the same kind as any of the specified furni info.
        /// </summary>
        public static IEnumerable<T> OfKinds<T>(this IEnumerable<T> items, IEnumerable<FurniInfo> furniInfo) where T : IItem
        {
            var set = new HashSet<(ItemType, int)>(furniInfo.Select(info => (info.Type, info.Kind)));
            return items.Where(item => set.Contains((item.Type, item.Kind)));
        }

        /// <summary>
        /// Selects items of the same kind as any of the specified furni info.
        /// </summary>
        public static IEnumerable<T> OfKinds<T>(this IEnumerable<T> items, params FurniInfo[] furniInfo)
            where T : IItem => OfKinds(items, (IEnumerable<FurniInfo>)furniInfo);

        /// <summary>
        /// Selects items of the specified category.
        /// </summary>
        public static IEnumerable<T> OfCategory<T>(this IEnumerable<T> items, FurniCategory category)
            where T : IItem => items.Where(x => FurniData.GetInfo(x)?.Category == category);

        /// <summary>
        /// Selects items with the specified furni identifier.
        /// </summary>
        public static IEnumerable<T> OfKind<T>(this IEnumerable<T> items, string identifier)
            where T : IItem => items.Where(x => FurniData.GetInfo(x)?.Identifier.Equals(identifier, StringComparison.OrdinalIgnoreCase) == true);

        /// <summary>
        /// Selects items with any of the specified furni identifiers.
        /// </summary>
        public static IEnumerable<T> OfKinds<T>(this IEnumerable<T> items, IEnumerable<string> identifiers)
            where T : IItem
        {
            HashSet<string> set = new(identifiers, StringComparer.OrdinalIgnoreCase);
            return items.Where(x => set.Contains(FurniData.GetInfo(x)?.Identifier ?? string.Empty));
        }

        /// <summary>
        /// Selects items with any of the specified furni identifiers.
        /// </summary>
        public static IEnumerable<T> OfKinds<T>(this IEnumerable<T> items, params string[] identifiers)
            where T : IItem => OfKinds(items, (IEnumerable<string>)identifiers);

        /// <summary>
        /// Selects items of the specified furni line.
        /// </summary>
        public static IEnumerable<T> OfLine<T>(this IEnumerable<T> items, string line)
            where T : IItem => items.Where(x => FurniData.GetInfo(x).Line.Equals(line, StringComparison.OrdinalIgnoreCase));

        /// <summary>
        /// Selects items with names matching the specified name.
        /// </summary>
        public static IEnumerable<T> Named<T>(this IEnumerable<T> items, string name)
            where T : IItem
        {
            return items.Where(x => FurniData.GetInfo(x)?.Name.Equals(name, StringComparison.OrdinalIgnoreCase) == true);
        }

        /// <summary>
        /// Selects items with names containing the specified search text.
        /// </summary>
        public static IEnumerable<T> NamedLike<T>(this IEnumerable<T> items, string searchText)
            where T : IItem
        {
            return items.Where(x => FurniData.GetInfo(x)?.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase) == true);
        }
        #endregion

        #region - Furni -
        /// <summary>
        /// Gets the area occupied by a floor item.
        /// </summary>
        public static Area GetArea<TFloorItem>(this TFloorItem item)
            where TFloorItem : IFloorItem
        {
            FurniInfo info = FurniData.GetInfo(item);
            Area area = new(item.XY, info.XDimension, info.YDimension);
            if (item.Direction % 4 == 2)
                area = area.Flip();
            return area;
        }

        /// <summary>
        /// Gets floor items intersecting the specified area.
        /// </summary>
        public static IEnumerable<TFloorItem> Intersecting<TFloorItem>(this IEnumerable<TFloorItem> items, Area area)
            where TFloorItem : IFloorItem
        {
            return items.Where(item => GetArea(item).Intersects(area));
        }

        /// <summary>
        /// Gets items of the specified kind.
        /// </summary>
        public static IEnumerable<T> OfKind<T>(this IEnumerable<T> items, int kind)
            where T : IFloorItem, IWallItem => items.Where(item => item.Kind == kind);

        /// <summary>
        /// Gets items of the specified kinds.
        /// </summary>
        public static IEnumerable<T> OfKinds<T>(this IEnumerable<T> items, IEnumerable<int> kinds)
            where T : IFloorItem, IWallItem   
        {
            HashSet<int> set = new(kinds);
            return items.Where(item => set.Contains(item.Kind));
        }

        /// <summary>
        /// Gets items of the specified kinds.
        /// </summary>
        public static IEnumerable<T> OfKinds<T>(this IEnumerable<T> items, params int[] kinds)
            where T : IFloorItem, IWallItem => OfKinds<T>(items, (IEnumerable<int>)kinds);

        /// <summary>
        /// Gets items owned by the specified user ID.
        /// </summary>
        public static IEnumerable<T> OwnedBy<T>(this IEnumerable<T> items, long ownerId)
            where T : IFurni => items.Where(item => item.OwnerId == ownerId);

        /// <summary>
        /// Gets items owned by the specified user.
        /// </summary>
        public static IEnumerable<T> OwnedBy<T>(this IEnumerable<T> items, string ownerName) where T : IFurni
            => items.Where(item => string.Equals(item.OwnerName, ownerName, StringComparison.OrdinalIgnoreCase));

        /// <summary>
        /// Gets wall items placed at the specified location.
        /// </summary>
        public static IEnumerable<TWallItem> At<TWallItem>(this IEnumerable<TWallItem> items,
            int? wallX = null, int? wallY = null, int? x = null, int? y = null,
            WallOrientation? orientation = null) where TWallItem : IWallItem
        {
            foreach (var item in items)
            {
                if (wallX.HasValue && item.WX != wallX) continue;
                if (wallY.HasValue && item.WY != wallY) continue;
                if (x.HasValue && item.LX != x) continue;
                if (y.HasValue && item.LY != y) continue;
                if (orientation.HasValue && item.Orientation != orientation) continue;
                yield return item;
            }
        }

        /// <summary>
        /// Gets wall items placed at the specified location.
        /// </summary>
        public static IEnumerable<TWallItem> At<TWallItem>(this IEnumerable<TWallItem> items, WallLocation location) where TWallItem : IWallItem
            => At(items, location.WX, location.WY, location.LX, location.LY, location.Orientation);
        #endregion

        #region - Inventory -
        /// <summary>
        /// Gets tradeable inventory items.
        /// </summary>
        public static IEnumerable<IInventoryItem> GetTradeable(this IEnumerable<IInventoryItem> items)
            => items.Where(item => item.IsTradeable);
        /// <summary>
        /// Gets sellable inventory items.
        /// </summary>
        public static IEnumerable<IInventoryItem> GetSellable(this IEnumerable<IInventoryItem> items)
            => items.Where(item => item.IsSellable);

        /// <summary>
        /// Splits inventory items into groups limited by the specified maximum slot and item counts.
        /// </summary>
        public static IEnumerable<IGrouping<int, IInventoryItem>> Group(this IEnumerable<IInventoryItem> items, int maxSlots = 9, int maxItems = 1500)
        {
            if (maxSlots < 1) throw new ArgumentOutOfRangeException(nameof(maxSlots));
            if (maxItems < 1) throw new ArgumentOutOfRangeException(nameof(maxItems));

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

                    if (currentSlots > maxSlots || currentCount > maxItems)
                    {
                        currentCount = 0;
                        currentSlots = 1;
                        return ++groupIndex;
                    }

                    return groupIndex;
                });
        }

        /// <summary>
        /// Groups the specified inventory items into fragments.
        /// </summary>
        /// <param name="items">The items to group into fragments.</param>
        /// <param name="size">The maximum number of items per fragment.</param>
        public static IEnumerable<InventoryFragment> Fragmentize(this IEnumerable<IInventoryItem> items, int size = 600)
        {
            if (items is not IInventoryItem[] array)
            {
                array = items.ToArray();
            }

            int currentIndex = 0;
            int totalFragments = (int)Math.Ceiling(array.Length / (double)size);

            foreach (var group in array.GroupBy(x => currentIndex++ / size))
            {
                yield return new InventoryFragment(group)
                {
                    Index = group.Key,
                    Total = totalFragments
                };
            }
        }
        #endregion

        #region - Rooms -
        public static IEnumerable<IRoomInfo> Filter(
            this IEnumerable<IRoomInfo> rooms,
            string? name = null,
            string? description = null,
            int? ownerId = null,
            string? owner = null,
            RoomAccess? access = null,
            TradePermissions? trading = null,
            RoomCategory? category = null,
            int? groupId = null,
            string? group = null)
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
                if (groupId.HasValue && (!roomInfo.IsGroupRoom || roomInfo.GroupId != groupId)) continue;
                if (group != null && (!roomInfo.IsGroupRoom || !roomInfo.GroupName.ToLower().Contains(group.ToLower()))) continue;
                yield return roomInfo;
            }
        }
        #endregion

        #region - Entities -
        /// <summary>
        /// Gets room entities at the specified X, Y, Z location and/or direction.
        /// </summary>
        public static IEnumerable<T> At<T>(this IEnumerable<T> entities,
            int? x = null, int? y = null, double? z = null, int? dir = null,
            double epsilon = XabboConst.DEFAULT_EPSILON) where T : IRoomEntity
        {
            foreach (var e in entities)
            {
                if (x.HasValue && e.Location.X != x) continue;
                if (y.HasValue && e.Location.Y != y) continue;
                if (z.HasValue && Math.Abs(e.Location.Z - z.Value) >= epsilon) continue;
                if (dir.HasValue && e.Direction != dir.Value) continue;
                yield return e;
            }
        }

        /// <summary>
        /// Gets entities at the specified X/Y location and optionally direction.
        /// </summary>
        public static IEnumerable<T> At<T>(this IEnumerable<T> entities,
            (int X, int Y) location, int? dir = null) where T : IRoomEntity
        {
            return At(entities, location.X, location.Y, null, dir);
        }

        /// <summary>
        /// Gets entities at the specified X/Y/Z location and optionally direction.
        /// </summary>
        public static IEnumerable<T> At<T>(this IEnumerable<T> entities,
            (int X, int Y, double Z) location, int? dir = null,
            double epsilon = XabboConst.DEFAULT_EPSILON) where T : IRoomEntity
        {
            return At(entities, location.X, location.Y, location.Z, dir, epsilon);
        }

        static void Test()
        {
            IEnumerable<IRoomUser> ents = Enumerable.Empty<IRoomUser>();
            ents.At(dir: 4);
        }

        public static IEnumerable<T> Inside<T>(this IEnumerable<T> entities, Area area)
            where T : IRoomEntity
        {
            return entities.Where(x => area.Contains(x.Location));
        }

        public static IEnumerable<T> Inside<T>(this IEnumerable<T> entities, AreaCollection areas)
            where T : IEntity
        {
            return entities.Where(x => areas.Contains(x.Location));
        }

        public static IEnumerable<T> Outside<T>(this IEnumerable<T> entities, Area area)
            where T : IEntity
        {
            return entities.Where(x => !area.Contains(x.Location));
        }

        public static IEnumerable<T> Outside<T>(this IEnumerable<T> entities, AreaCollection areas)
            where T : IEntity
        {
            return entities.Where(x => !areas.Contains(x.Location));
        }
        #endregion
    }
}