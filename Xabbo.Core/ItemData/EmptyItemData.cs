using System;

using Xabbo.Messages;

namespace Xabbo.Core
{
    public class EmptyItemData : ItemData, IEmptyItemData
    {
        public EmptyItemData()
            : base(ItemDataType.Empty)
        { }

        public EmptyItemData(IEmptyItemData data)
            : this()
        { }

        protected override void WriteData(IPacket packet) { }
    }
}
