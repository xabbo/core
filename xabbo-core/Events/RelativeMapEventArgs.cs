using System;

namespace Xabbo.Core.Events
{
    public class HeightMapEventArgs : EventArgs
    {
        public HeightMap Map { get; }

        public HeightMapEventArgs(HeightMap map)
        {
            Map = map;
        }
    }
}
