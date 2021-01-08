using System;

namespace Xabbo.Core.Protocol
{
    public interface IPacketData
    {
        void Write(IPacket packet);
    }
}
