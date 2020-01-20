using System;
using System.Threading.Tasks;

using Xabbo.Core.Messages;
using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public interface IInterceptor
    {
        MessageDispatcher Dispatcher { get; }
        Task<bool> SendToServerAsync(short header, params object[] values);
        Task<bool> SendToServerAsync(Packet packet);
        Task<bool> SendToServerAsync(byte[] data);
        Task<bool> SendToClientAsync(short header, params object[] values);
        Task<bool> SendToClientAsync(Packet packet);
        Task<bool> SendToClientAsync(byte[] data);
    }
}
