using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Xabbo.Core.GameData
{
    public class FurniData : IReadOnlyCollection<FurniInfo>
    {
        public static FurniData LoadJson(string json) => new(Json.FurniData.Load(json));
        public static FurniData LoadXml(Stream stream) => new(Xml.FurniData.Load(stream));
        public static FurniData LoadXml(string path)
        {
            using Stream stream = File.OpenRead(path);
            return LoadXml(stream);
        }

        private readonly IReadOnlyDictionary<string, FurniInfo> _identifierMap;
        private readonly IReadOnlyDictionary<int, FurniInfo> _floorItemMap, _wallItemMap;

        /// <summary>
        /// Gets the information of all floor items.
        /// </summary>
        public IReadOnlyCollection<FurniInfo> FloorItems { get; }

        /// <summary>
        /// Gets the information of all wall items.
        /// </summary>
        public IReadOnlyCollection<FurniInfo> WallItems { get; }

        /// <summary>
        /// Gets the total number of <see cref="FurniInfo"/> contained in the furni data.
        /// </summary>
        public int Count => FloorItems.Count + WallItems.Count;

        public IEnumerator<FurniInfo> GetEnumerator() => FloorItems.Concat(WallItems).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Gets the information of the furni with the specified identifier, or <c>null</c> if it does not exist.
        /// </summary>
        public FurniInfo? this[string identifier] => GetInfo(identifier);

        internal FurniData(Xml.FurniData proxy)
        {
            FloorItems = proxy.FloorItems
                .Select(furniInfoProxy => new FurniInfo(ItemType.Floor, furniInfoProxy))
                .ToList().AsReadOnly();

            WallItems = proxy.WallItems
                .Select(furniInfoProxy => new FurniInfo(ItemType.Wall, furniInfoProxy))
                .ToList().AsReadOnly();

            _identifierMap = this.ToDictionary(furniInfo => furniInfo.Identifier, StringComparer.InvariantCultureIgnoreCase);
            _floorItemMap = FloorItems.ToDictionary(furniInfo => furniInfo.Kind);
            _wallItemMap = WallItems.ToDictionary(wallItem => wallItem.Kind);
        }

        internal FurniData(Json.FurniData proxy)
        {
            FloorItems = proxy.RoomItemTypes.FurniType
                .Select(furniInfoProxy => new FurniInfo(ItemType.Floor, furniInfoProxy))
                .ToList().AsReadOnly();

            WallItems = proxy.WallItemTypes.FurniType
                .Select(furniInfoProxy => new FurniInfo(ItemType.Wall, furniInfoProxy))
                .ToList().AsReadOnly();

            _identifierMap = this.ToDictionary(furniInfo => furniInfo.Identifier, StringComparer.InvariantCultureIgnoreCase);
            _floorItemMap = FloorItems.ToDictionary(furniInfo => furniInfo.Kind);
            _wallItemMap = WallItems.ToDictionary(wallItem => wallItem.Kind);
        }

        /// <summary>
        /// Gets the information of the furni with the specified type and kind, or <c>null</c> if it does not exist.
        /// </summary>
        public FurniInfo? GetInfo(ItemType type, int kind)
        {
            if (type == ItemType.Floor)
                return GetFloorItem(kind);
            else if (type == ItemType.Wall)
                return GetWallItem(kind);
            else
                return null;
        }

        /// <summary>
        /// Gets the information of the specified item, or <c>null</c> if it does not exist.
        /// </summary>
        public FurniInfo? GetInfo(IItem item) => GetInfo(item.Type, item.Kind);

        /// <summary>
        /// Gets the information of the furni with the specified identifier, or <c>null</c> if it does not exist.
        /// </summary>
        public FurniInfo? GetInfo(string identifier) => _identifierMap.TryGetValue(identifier, out FurniInfo? info) ? info : null;

        /// <summary>
        /// Gets the information for the floor item of the specified kind, or <c>null</c> if it does not exist.
        /// </summary>
        public FurniInfo? GetFloorItem(int kind) => _floorItemMap.TryGetValue(kind, out FurniInfo? furniInfo) ? furniInfo : null;

        /// <summary>
        /// Gets the information for the wall item of the specified kind, or <c>null</c> if it does not exist.
        /// </summary>
        public FurniInfo? GetWallItem(int kind) => _wallItemMap.TryGetValue(kind, out FurniInfo? furniInfo) ? furniInfo : null;

        private static IEnumerable<FurniInfo> FindItems(IEnumerable<FurniInfo> infos, string searchText)
        {
            searchText = searchText.ToLower();
            return infos
                .Where(x => x.Name.ToLower().Contains(searchText))
                .OrderBy(x => Math.Abs(x.Name.Length - searchText.Length));
        }

        /// <summary>
        /// Finds information of furni containing the specified text in its name.
        /// </summary>
        public IEnumerable<FurniInfo> FindItems(string searchText) => FindItems(this, searchText);

        /// <summary>
        /// Finds information of floor furni containing the specified text in its name.
        /// </summary>
        public IEnumerable<FurniInfo> FindFloorItems(string searchText) => FindItems(FloorItems, searchText);

        /// <summary>
        /// Finds information of wall furni containing the specified text in its name.
        /// </summary>
        public IEnumerable<FurniInfo> FindWallItems(string searchText) => FindItems(WallItems, searchText);

        /// <summary>
        /// Finds the information of a furni containing the specified text in its name.
        /// </summary>
        public FurniInfo? FindItem(string searchText) => FindItems(this, searchText).FirstOrDefault();

        /// <summary>
        /// Finds the information of a floor furni containing the specified text in its name.
        /// </summary>
        public FurniInfo? FindFloorItem(string searchText) => FindItems(FloorItems, searchText).FirstOrDefault();

        /// <summary>
        /// Finds the information of a wall furni containing the specified text in its name.
        /// </summary>
        public FurniInfo? FindWallItem(string searchText) => FindItems(WallItems, searchText).FirstOrDefault();

        public ItemDescriptor GetItemDescriptor(IItem item)
        {
            string variant = string.Empty;

            var info = GetInfo(item) ?? throw new ArgumentException(
                $"Unable to find furni info for {item.Type.ToString().ToLower()} item {item.Kind}."
            );

            if (info.Identifier == "poster")
            {
                if (item is IInventoryItem inventoryItem)
                {
                    variant = inventoryItem.Data.Value;
                }
                else if (item is IWallItem wallItem)
                {
                    variant = wallItem.Data;
                }
                else if (item is ICatalogProduct catalogProduct)
                {
                    variant = catalogProduct.Variant;
                }
                else
                {
                    throw new ArgumentException($"Unable to find variant for poster of item type: {item.GetType().FullName}.");
                }
            }

            return new ItemDescriptor(item.Type, item.Kind, variant);
        }

        public StackDescriptor GetStackDescriptor(IInventoryItem item)
        {
            string variant = string.Empty;

            var info = GetInfo(item) ?? throw new ArgumentException(
                $"Unable to find furni info for {item.Type.ToString().ToLower()} item {item.Kind}."
            );

            if (info.Identifier == "poster")
            {
                variant = item.Data.Value;
            }

            return new StackDescriptor(item.Type, item.Kind, variant, item.IsTradeable, item.IsGroupable);
        }

        /// <summary>
        /// Groups items by maximum slots/items in order to offer in a trade.
        /// </summary>
        public IEnumerable<IGrouping<int, IInventoryItem>> GroupItems(IEnumerable<IInventoryItem> items,
            int maxSlots = 9, int maxItems = 1500)
        {
            if (maxSlots < 1 || maxSlots > 9) throw new ArgumentOutOfRangeException(nameof(maxSlots));
            if (maxItems < 1 || maxItems > 1500) throw new ArgumentOutOfRangeException(nameof(maxItems));

            StackDescriptor currentDescriptor = default;
            int currentGroup = 0, currentSlots = 0, currentItems = 0;

            return items
                .OrderBy(item => item.Type)
                .ThenBy(item => item.Kind)
                .ThenBy(item => item.Data?.Value)
                .GroupBy(item =>
                {
                    var descriptor = GetStackDescriptor(item);
                    if (!item.IsGroupable || descriptor != currentDescriptor)
                        currentSlots++;

                    currentDescriptor = descriptor;
                    currentItems++;

                    if (currentSlots > maxSlots || currentItems > maxItems)
                    {
                        currentDescriptor = default;
                        currentSlots = currentItems = 0;
                        currentGroup++;
                    }

                    return currentGroup;
                });
        }
    }
}