using System;
using System.Runtime.CompilerServices;

namespace Xabbo.Core;

internal static class EnumExtensions
{
    public static int AsInt32<TEnum>(this TEnum value)
        where TEnum : unmanaged, Enum
    {
        if (Unsafe.SizeOf<TEnum>() != Unsafe.SizeOf<int>())
            throw new ArgumentException("Enum size does not match size of int.");
        return Unsafe.As<TEnum, int>(ref value);
    }
}
