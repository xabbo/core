using System;
using System.Diagnostics;
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

        public async Task<TResult> ExecuteAsync(int timeout, CancellationToken token = default)
        {
            CancellationTokenSource cts = null;

            try
            {
                cts = CancellationTokenSource.CreateLinkedTokenSource(token);
                cts.Token.Register(() => SetCanceled());
                if (timeout > 0) cts.CancelAfter(timeout);

                Hook();

                return await ExecuteAsync().ConfigureAwait(false);
            }
            finally
            {
                Unhook();
                cts?.Cancel();
                cts?.Dispose();
            }
        }

        protected virtual void Hook()
        {
            if (Dispatcher.Attach(this))
            {
                Debug.WriteLine($"[{GetType().Name}.Hook] attached listener");
            }
            else
            {
                Debug.WriteLine($"[{GetType().Name}.Hook] no listener methods were attached");
            }
        }

        protected virtual void Unhook()
        {
            if (Dispatcher.Detach(this))
            {
                Debug.WriteLine($"[{GetType().Name}.Unhook] detached listener");
            }
            else
            {
                Debug.WriteLine($"[{GetType().Name}.Unhook] failed to detach listener");
            }
        }

        protected virtual async Task<TResult> ExecuteAsync()
        {
            await OnExecuteAsync().ConfigureAwait(false);
            return await completion.Task.ConfigureAwait(false);
        }

        protected abstract Task OnExecuteAsync();

        protected bool SetResult(TResult result) => completion.TrySetResult(result);
        protected bool SetCanceled() => completion.TrySetCanceled();
        protected bool SetException(Exception ex) => completion.TrySetException(ex);

        protected Task<bool> SendAsync(short header, params object[] values) => interceptor.SendToServerAsync(header, values);
        protected Task<bool> SendAsync(Packet packet) => interceptor.SendToServerAsync(packet);
        protected Task<bool> SendAsync(byte[] data) => interceptor.SendToServerAsync(data);

        protected Task<bool> SendToClientAsync(short header, params object[] values) => interceptor.SendToClientAsync(header, values);
        protected Task<bool> SendToClientAsync(Packet packet) => interceptor.SendToClientAsync(packet);
        protected Task<bool> SendToClientAsync(byte[] data) => interceptor.SendToClientAsync(data);
    }
}
