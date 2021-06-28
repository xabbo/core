using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Xabbo.Core
{
    public class AreaCollection : ICollection<Area>
    {
        private readonly HashSet<Area> _areas = new();

        public int Count => _areas.Count;
        public bool IsReadOnly => false;

        public IReadOnlyList<(int X, int Y)> Tiles { get; private set; } = Array.Empty<(int, int)>();

        public AreaCollection() { }

        public AreaCollection(params Area[] areas)
            : this((IEnumerable<Area>)areas)
        { }

        public AreaCollection(IEnumerable<Area> areas)
        {
            _areas = new HashSet<Area>(areas);
            UpdateTiles();
        }

        public bool Contains(Tile tile) => Contains(tile.X, tile.Y);
        public bool Contains((int X, int Y) position) => Contains(position.X, position.Y);
        public bool Contains(int x, int y) => _areas.Any(area => area.Contains(x, y));

        private void UpdateTiles() => Tiles = Area.GetAllPoints(_areas).AsReadOnly();

        public void Add(Area item)
        {
            if (_areas.Add(item))
                UpdateTiles();
        }

        public void Clear()
        {
            _areas.Clear();
            UpdateTiles();
        }

        public bool Contains(Area item) => _areas.Contains(item);

        public void CopyTo(Area[] array, int arrayIndex) => _areas.CopyTo(array, arrayIndex);

        public IEnumerator<Area> GetEnumerator() => _areas.GetEnumerator();

        public bool Remove(Area item)
        {
            bool removed = _areas.Remove(item);
            if (removed) UpdateTiles();
            return removed;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
