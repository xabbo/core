using System;
using System.Threading;
using System.Threading.Tasks;

using Xabbo.Core.Messages;
using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public abstract class InterceptorTask<TResult> : IListener
    {
        private readonly IInterceptor interceptor;
        private readonly TaskCompletionSource<TResult> completion;

        protected MessageDispatcher Dispatcher => interceptor.Dispatcher;
        protected IncomingHeaders In => Dispatcher.Headers.Incoming;
        protected OutgoingHeaders Out => Dispatcher.Headers.Outgoing;

        protected InterceptorTask(IInterceptor interceptor)
        {
            this.interceptor = interceptor;
            completion = new TaskCompletionSource<TResult>();
        }

        public TResult Execute(int timeout, CancellationToken token) => ExecuteAsync(timeout, token).GetAwaiter().GetResult();

        public async Task<TResult> ExecuteAsync(int timeout, CancellationToken token)
        {
            CancellationTokenSource cts = null;

            try
            {
                cts = CancellationTokenSource.CreateLinkedTokenSource(token);
                cts.Token.Register(() => SetCanceled());
                if (timeout > 0) cts.CancelAfter(timeout);

                Hook();

                return await ExecuteAsync();
            }
            finally
            {
                Unhook();
                cts?.Cancel();
                cts?.Dispose();
            }
        }

        protected virtual void Hook() => Dispatcher.AttachListener(this);
        protected virtual void Unhook() => Dispatcher.DetachListener(this);

        protected virtual async Task<TResult> ExecuteAsync()
        {
            await OnExecuteAsync();
            return await completion.Task;
        }

        protected abstract Task OnExecuteAsync();

        protected void SetResult(TResult result) => completion.TrySetResult(result);
        protected void SetCanceled() => completion.TrySetCanceled();
        protected void SetException(Exception ex) => completion.TrySetException(ex);

        protected Task<bool> SendAsync(short header, params object[] values) => interceptor.SendToServerAsync(header, values);
        protected Task<bool> SendAsync(Packet packet) => interceptor.SendToServerAsync(packet);
        protected Task<bool> SendAsync(byte[] data) => interceptor.SendToServerAsync(data);

        protected Task<bool> SendToClientAsync(short header, params object[] values) => interceptor.SendToClientAsync(header, values);
        protected Task<bool> SendToClientAsync(Packet packet) => interceptor.SendToClientAsync(packet);
        protected Task<bool> SendToClientAsync(byte[] data) => interceptor.SendToClientAsync(data);
    }
}
