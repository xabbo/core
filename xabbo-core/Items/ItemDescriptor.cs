using System;

namespace Xabbo.Core
{
    public struct ItemDescriptor
    {
        public ItemType Type { get; }
        public int Kind { get; }
        public string Variation { get; }

        public ItemDescriptor(ItemType type, int kind)
            : this(type, kind, null)
        { }

        public ItemDescriptor(ItemType type, int kind, string variation)
        {
            Type = type;
            Kind = kind;

            if (string.IsNullOrWhiteSpace(variation))
                Variation = string.Empty;
            else
                Variation = variation;
        }

        public override int GetHashCode() => (Type, Kind, Variation).GetHashCode();

        public override bool Equals(object obj)
        {
            return obj is ItemDescriptor other
                && Equals(other);
        }

        public bool Equals(ItemDescriptor other)
        {
            return
                Type == other.Type &&
                Kind == other.Kind &&
                Variation == other.Variation;
        }
    }
}
