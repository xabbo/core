using System;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class ItemData4 : ItemData
    {
        public ItemData4()
            : base(ItemDataType.ItemData4)
        { }

        protected override void WriteData(Packet packet) { }
    }
}
