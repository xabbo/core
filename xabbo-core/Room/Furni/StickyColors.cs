using System;
using System.Collections;
using System.Collections.Generic;

namespace Xabbo.Core
{
    public sealed class StickyColors : IReadOnlyList<string>
    {
        public const string Blue = "9CCEFF";
        public const string Pink = "FF9CFF";
        public const string Green = "9CFF9C";
        public const string Yellow = "FFFF33";

        private readonly List<string> colors;

        internal StickyColors()
        {
            colors = new List<string>()
            {
                Blue,
                Pink,
                Green,
                Yellow
            };
        }

        public string this[int index] => colors[index];
        public int Count => colors.Count;
        public IEnumerator<string> GetEnumerator() => colors.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
