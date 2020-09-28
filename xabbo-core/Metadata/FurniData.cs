using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Xabbo.Core.Metadata
{
    public class FurniData : IEnumerable<FurniInfo>
    {
        public static FurniData Load(Stream stream) => new FurniData(FurniDataXml.Load(stream));
        public static FurniData Load(string path)
        {
            using (var stream = File.OpenRead(path))
                return Load(stream);
        }

        private readonly IReadOnlyDictionary<string, FurniInfo> classNameDictionary;
        private readonly IReadOnlyDictionary<int, FurniInfo> floorItemDictionary, wallItemDictionary;

        /// <summary>
        /// Gets the information of all floor items.
        /// </summary>
        public IReadOnlyList<FurniInfo> FloorItems { get; }

        /// <summary>
        /// Gets the information of all wall items.
        /// </summary>
        public IReadOnlyList<FurniInfo> WallItems { get; }

        public IEnumerator<FurniInfo> GetEnumerator() => FloorItems.Concat(WallItems).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Gets the information of the furni with the specified class name, or <c>null</c> if it does not exist.
        /// </summary>
        public FurniInfo this[string className] => classNameDictionary.TryGetValue(className.ToLower(), out FurniInfo info) ? info : null;

        internal FurniData(FurniDataXml proxy)
        {
            FloorItems = proxy.FloorItems
                .Select(furniInfoProxy => new FurniInfo(ItemType.Floor, furniInfoProxy))
                .ToList().AsReadOnly();

            WallItems = proxy.WallItems
                .Select(furniInfoProxy => new FurniInfo(ItemType.Wall, furniInfoProxy))
                .ToList().AsReadOnly();

            classNameDictionary = this.ToDictionary(furniInfo => furniInfo.ClassName.ToLower());
            floorItemDictionary = FloorItems.ToDictionary(furniInfo => furniInfo.Id);
            wallItemDictionary = WallItems.ToDictionary(wallItem => wallItem.Id);
        }

        /// <summary>
        /// Gets the information of the furni with the specified type and kind, or <c>null</c> if it does not exist.
        /// </summary>
        public FurniInfo GetInfo(ItemType type, int kind)
        {
            if (type == ItemType.Floor)
                return GetFloorItem(kind);
            else if (type == ItemType.Wall)
                return GetWallItem(kind);
            else
                return null;
        }

        /// <summary>
        /// Gets the information for the furni of the specified item.
        /// </summary>
        public FurniInfo GetInfo(IItem item) => GetInfo(item.Type, item.Kind);

        /// <summary>
        /// Gets the information for the floor item of the specified kind.
        /// </summary>
        public FurniInfo GetFloorItem(int kind) => floorItemDictionary.TryGetValue(kind, out FurniInfo furniInfo) ? furniInfo : null;

        /// <summary>
        /// Gets the information for the wall item of the specified kind.
        /// </summary>
        public FurniInfo GetWallItem(int kind) => wallItemDictionary.TryGetValue(kind, out FurniInfo furniInfo) ? furniInfo : null;

        private IEnumerable<FurniInfo> FindItems(IEnumerable<FurniInfo> infos, string searchText)
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
        public FurniInfo FindItem(string name) => FindItems(this, name).FirstOrDefault();

        /// <summary>
        /// Finds the information of a floor furni containing the specified text in its name.
        /// </summary>
        public FurniInfo FindFloorItem(string name) => FindItems(FloorItems, name).FirstOrDefault();

        /// <summary>
        /// Finds the information of a wall furni containing the specified text in its name.
        /// </summary>
        public FurniInfo FindWallItem(string name) => FindItems(WallItems, name).FirstOrDefault();
    }
}