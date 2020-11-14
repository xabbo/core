using System;

namespace Xabbo.Core
{
    public struct WallOrientation
    {
        public static readonly WallOrientation Left = new WallOrientation('l');
        public static readonly WallOrientation Right = new WallOrientation('r');

        public char Value { get; }

        public bool IsLeft => Value == 'l';
        public bool IsRight => Value == 'r';

        private WallOrientation(char value)
        {
            Value = value;
        }

        public static WallOrientation FromChar(char c)
        {
            switch (c)
            {
                case 'l':
                case 'L':
                    return Left;
                case 'r':
                case 'R':
                    return Right;
                default: throw new InvalidCastException($"Invalid wall orientation '{c}', must be 'l' or 'r'");
            }
        }

        public static implicit operator WallOrientation(char c) => FromChar(c);
        public static implicit operator char(WallOrientation orientation) => orientation.Value;
    }
}
