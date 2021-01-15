using System;

namespace Xabbo.Core.Messages
{
    public interface IPacketData
    {
        void Write(IPacket packet);
    }
}
