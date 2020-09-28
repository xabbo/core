using System;

namespace Xabbo.Core.Events
{
    public class HeightMapEventArgs : EventArgs
    {
        public IHeightMap Map { get; }

        public HeightMapEventArgs(IHeightMap map)
        {
            Map = map;
        }
    }
}
