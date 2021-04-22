using System;

namespace Xabbo.Core.Events
{
    public class HeightMapEventArgs : EventArgs
    {
        public IHeightmap Map { get; }

        public HeightMapEventArgs(IHeightmap map)
        {
            Map = map;
        }
    }
}
