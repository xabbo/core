﻿using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Xabbo.Core.GameData
{
    public class FurniData : IReadOnlyCollection<FurniInfo>
    {
        public static FurniData LoadJson(string json) => new(Json.FurniData.Load(json));
        public static FurniData LoadJsonFile(string path) => LoadJson(File.ReadAllText(path));
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
        /// Gets the information of the furni with the specified identifier.
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
        /// Returns whether furni info with the specified type and kind exists or not.
        /// </summary>
        public bool Exists(ItemType type, int kind)
        {
            return type switch
            {
                ItemType.Floor => _floorItemMap.ContainsKey(kind),
                ItemType.Wall => _wallItemMap.ContainsKey(kind),
                _ => false
            };
        }
        /// <summary>
        /// Returns whether furni info for the specified item exists or not.
        /// </summary>
        public bool Exists(IItem item) => Exists(item.Type, item.Kind);
        /// <summary>
        /// Returns whether floor furni info with the specified kind exists or not.
        /// </summary>
        public bool FloorItemExists(int kind) => Exists(ItemType.Floor, kind);
        /// <summary>
        /// Returns whether wall furni info with the specified kind exists or not.
        /// </summary>
        public bool WallItemExists(int kind) => Exists(ItemType.Wall, kind);

        /// <summary>
        /// Gets the information of the furni with the specified type and kind.
        /// </summary>
        public FurniInfo GetInfo(ItemType type, int kind) => type switch
        {
            ItemType.Floor => GetFloorItem(kind),
            ItemType.Wall => GetWallItem(kind),
            _ => throw new Exception($"Invalid item type specified: {type}.")
        };

        /// <summary>
        /// Gets the furni info of the specified item.
        /// </summary>
        public FurniInfo GetInfo(IItem item) => GetInfo(item.Type, item.Kind);

        /// <summary>
        /// Gets the furni info of the furni with the specified identifier.
        /// </summary>
        public FurniInfo? GetInfo(string identifier) => _identifierMap.TryGetValue(identifier, out FurniInfo? info) ? info : null;

        /// <summary>
        /// Gets the furni info for the floor item of the specified kind.
        /// </summary>
        public FurniInfo GetFloorItem(int kind)
        {
            if (!_floorItemMap.TryGetValue(kind, out FurniInfo? furniInfo))
                throw new Exception($"Failed to find furni info for item: Floor/{kind}.");

            return furniInfo;
        }

        /// <summary>
        /// Gets the furni info for the wall item of the specified kind.
        /// </summary>
        public FurniInfo GetWallItem(int kind)
        {
            if (!_wallItemMap.TryGetValue(kind, out FurniInfo? furniInfo))
                throw new Exception($"Failed to find furni info for item: Wall/{kind}.");

            return furniInfo;
        }

        private static IEnumerable<FurniInfo> FindItems(IEnumerable<FurniInfo> infos, string searchText)
        {
            searchText = searchText.ToLower();
            return infos
                .Where(x => x.Name.ToLower().Contains(searchText))
                .OrderBy(x => Math.Abs(x.Name.Length - searchText.Length));
        }

        /// <summary>
        /// Finds information of all furni containing the specified text in its name.
        /// </summary>
        public IEnumerable<FurniInfo> FindItems(string searchText) => FindItems(this, searchText);

        /// <summary>
        /// Finds information of all floor furni containing the specified text in its name.
        /// </summary>
        public IEnumerable<FurniInfo> FindFloorItems(string searchText) => FindItems(FloorItems, searchText);

        /// <summary>
        /// Finds information of all wall furni containing the specified text in its name.
        /// </summary>
        public IEnumerable<FurniInfo> FindWallItems(string searchText) => FindItems(WallItems, searchText);

        /// <summary>
        /// Finds the first information of a furni containing the specified text in its name,
        /// or <c>null</c> if no matches were found.
        /// </summary>
        public FurniInfo? FindItem(string searchText) => FindItems(this, searchText).FirstOrDefault();

        /// <summary>
        /// Finds the first information of a floor furni containing the specified text in its name,
        /// or <c>null</c> if no matches were found.
        /// </summary>
        public FurniInfo? FindFloorItem(string searchText) => FindItems(FloorItems, searchText).FirstOrDefault();

        /// <summary>
        /// Finds the first information of a wall furni containing the specified text in its name,
        /// or <c>null</c> if no matches were found.
        /// </summary>
        public FurniInfo? FindWallItem(string searchText) => FindItems(WallItems, searchText).FirstOrDefault();

        /// <summary>
        /// Gets the <see cref="ItemDescriptor"/> of the specified item.
        /// </summary>
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