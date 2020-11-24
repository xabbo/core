using System;

namespace Xabbo.Core
{
    public struct StackDescriptor
    {
        public ItemType Type { get; }
        public int Kind { get; }
        public string Variant { get; }
        public bool IsTradeable { get; }
        public bool IsGroupable { get; }

        public StackDescriptor(ItemType type, int kind, string variant,
            bool isTradeable, bool isGroupable)
        {
            Type = type;
            Kind = kind;
            Variant = variant ?? string.Empty;
            IsTradeable = isTradeable;
            IsGroupable = isGroupable;
        }

        public override int GetHashCode() => (Type, Kind, Variant, IsTradeable, IsGroupable).GetHashCode();

        public override bool Equals(object obj)
        {
            return obj is StackDescriptor other
                && Equals(other);
        }

        public bool Equals(StackDescriptor other)
        {
            return
                Type == other.Type &&
                Kind == other.Kind &&
                Variant == other.Variant &&
                IsTradeable == other.IsTradeable &&
                IsGroupable == other.IsGroupable;
        }

        public static bool operator ==(StackDescriptor a, StackDescriptor b) => a.Equals(b);
        public static bool operator !=(StackDescriptor a, StackDescriptor b) => !(a == b);
    }
}
