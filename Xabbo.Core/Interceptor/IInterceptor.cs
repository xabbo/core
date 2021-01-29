using System;
using System.Threading.Tasks;

using Xabbo.Core.Messages;
using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public interface IInterceptor
    {
        MessageDispatcher Dispatcher { get; }
        Task<bool> SendToServerAsync(Header header, params object[] values);
        Task<bool> SendToServerAsync(IReadOnlyPacket packet);
        Task<bool> SendToClientAsync(Header header, params object[] values);
        Task<bool> SendToClientAsync(IReadOnlyPacket packet);
    }
}
