using System;

namespace Xabbo.Core.Protocol
{
    public interface IWritable
    {
        void Write(Packet packet);
    }
}
