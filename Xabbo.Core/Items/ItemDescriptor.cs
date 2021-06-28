using System;

namespace Xabbo.Core
{
    public struct ItemDescriptor
    {
        public ItemType Type { get; }
        public int Kind { get; }
        public string Variant { get; }

        public ItemDescriptor(ItemType type, int kind)
            : this(type, kind, null)
        { }

        public ItemDescriptor(ItemType type, int kind, string? variant)
        {
            Type = type;
            Kind = kind;
            Variant = variant ?? string.Empty;
        }

        public override int GetHashCode() => (Type, Kind, Variant).GetHashCode();

        public override bool Equals(object? obj)
        {
            return obj is ItemDescriptor other
                && Equals(other);
        }

        public bool Equals(ItemDescriptor other)
        {
            return
                Type == other.Type &&
                Kind == other.Kind &&
                Variant == other.Variant;
        }

        public static bool operator ==(ItemDescriptor a, ItemDescriptor b) => a.Equals(b);
        public static bool operator !=(ItemDescriptor a, ItemDescriptor b) => !(a == b);
    }
}
