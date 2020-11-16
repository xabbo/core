﻿using System;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class LegacyData : ItemData, ILegacyData
    {
        public LegacyData()
            : base(ItemDataType.Legacy)
        { }

        protected override void Initialize(IReadOnlyPacket packet)
        {
            LegacyString = packet.ReadString();
            base.Initialize(packet);
        }

        protected override void WriteData(IPacket packet)
        {
            packet.WriteString(LegacyString);
            WriteBase(packet);
        }
    }
}