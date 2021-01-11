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

        public override string ToString() => Value.ToString();

        public static WallOrientation FromChar(char c)
        {
            return c switch
            {
                'l' or 'L' => Left,
                'r' or 'R' => Right,
                _ => throw new InvalidCastException($"Invalid wall orientation '{c}', must be 'l' or 'r'"),
            };
        }

        public static implicit operator WallOrientation(char c) => FromChar(c);
        public static implicit operator char(WallOrientation orientation) => orientation.Value;
    }
}
