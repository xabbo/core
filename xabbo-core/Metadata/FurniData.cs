using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Collections;

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

        private readonly IReadOnlyDictionary<int, FurniInfo> floorItemDictionary, wallItemDictionary;

        public IReadOnlyList<FurniInfo> FloorItems { get; }
        public IReadOnlyList<FurniInfo> WallItems { get; }
        public IEnumerator<FurniInfo> GetEnumerator() => FloorItems.Concat(WallItems).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        internal FurniData(FurniDataXml proxy)
        {
            FloorItems = proxy.FloorItems
                .Select(furniInfoProxy => new FurniInfo(FurniType.Floor, furniInfoProxy))
                .ToList().AsReadOnly();

            WallItems = proxy.WallItems
                .Select(furniInfoProxy => new FurniInfo(FurniType.Wall, furniInfoProxy))
                .ToList().AsReadOnly();

            var dict = new Dictionary<int, FurniInfo>();

            floorItemDictionary = FloorItems.ToDictionary(furniInfo => furniInfo.Id);
            wallItemDictionary = WallItems.ToDictionary(wallItem => wallItem.Id);
        }

        public FurniInfo GetInfo(FurniType type, int kind)
        {
            if (type == FurniType.Floor)
                return GetFloorItem(kind);
            else if (type == FurniType.Wall)
                return GetWallItem(kind);
            else
                return null;
        }
        public FurniInfo GetInfo(IItem item) => GetInfo(item.Type, item.Kind);

        public FurniInfo GetFloorItem(int kind) => floorItemDictionary.TryGetValue(kind, out FurniInfo furniInfo) ? furniInfo : null;
        public FurniInfo GetWallItem(int kind) => wallItemDictionary.TryGetValue(kind, out FurniInfo furniInfo) ? furniInfo : null;

        private IEnumerable<FurniInfo> FindItems(IEnumerable<FurniInfo> infos, string name)
        {
            name = name.ToLower();
            return infos
                .Where(x => x.Name.ToLower().Contains(name))
                .OrderBy(x => Math.Abs(x.Name.Length - name.Length));
        }
        public IEnumerable<FurniInfo> FindItems(string name) => FindItems(this, name);
        public IEnumerable<FurniInfo> FindFloorItems(string name) => FindItems(FloorItems, name);
        public IEnumerable<FurniInfo> FindWallItems(string name) => FindItems(WallItems, name);
        public FurniInfo FindItem(string name) => FindItems(this, name).FirstOrDefault();
        public FurniInfo FindFloorItem(string name) => FindItems(FloorItems, name).FirstOrDefault();
        public FurniInfo FindWallItem(string name) => FindItems(WallItems, name).FirstOrDefault();

        public FurniInfo GetInfo(string className)
            => this.FirstOrDefault(info => info.ClassName.Equals(className, StringComparison.InvariantCultureIgnoreCase));
    }
}