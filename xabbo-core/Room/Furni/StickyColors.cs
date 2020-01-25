using System;
using System.Collections;
using System.Collections.Generic;

namespace Xabbo.Core
{
    public sealed class StickyColors : IReadOnlyList<StickyColor>
    {
        public static readonly StickyColor Blue = new StickyColor("Blue", "9CCEFF");
        public static readonly StickyColor Pink = new StickyColor("Pink", "FF9CFF");
        public static readonly StickyColor Green = new StickyColor("Green", "9CFF9C");
        public static readonly StickyColor Yellow = new StickyColor("Yellow", "FFFF33");

        private readonly List<StickyColor> colors;

        internal StickyColors()
        {
            colors = new List<StickyColor>()
            {
                Blue,
                Pink,
                Green,
                Yellow
            };
        }

        public StickyColor this[int index] => colors[index];
        public int Count => colors.Count;
        public IEnumerator<StickyColor> GetEnumerator() => colors.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
