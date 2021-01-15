using System;
using System.Threading;
using System.Threading.Tasks;

using Xabbo.Core.Messages;
using Xabbo.Core.Messages;

namespace Xabbo.Core.Tasks
{
    public abstract class InterceptorTask<TResult> : IListener
    {
        private readonly SemaphoreSlim executionSemaphore = new SemaphoreSlim(1);
        private readonly TaskCompletionSource<TResult> completion;

        private readonly IInterceptor interceptor;
        protected MessageDispatcher Dispatcher => interceptor.Dispatcher;
        protected Incoming In => Dispatcher.Headers.Incoming;
        protected Outgoing Out => Dispatcher.Headers.Outgoing;

        protected InterceptorTask(IInterceptor interceptor)
        {
            this.interceptor = interceptor;
            completion = new TaskCompletionSource<TResult>();
        }

        public TResult Execute(int timeout, CancellationToken token) => ExecuteAsync(timeout, token).GetAwaiter().GetResult();

        public async Task<TResult> ExecuteAsync(int timeout, CancellationToken token)
        {
            if (!executionSemaphore.Wait(0))
                throw new InvalidOperationException("The interceptor task has already been executed.");

            CancellationTokenSource cts = null;

            try
            {
                cts = CancellationTokenSource.CreateLinkedTokenSource(token);
                cts.Token.Register(() => SetCanceled());
                if (timeout > 0) cts.CancelAfter(timeout);

                Hook();

                return await ExecuteAsync().ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            when (!token.IsCancellationRequested)
            {
                throw;
                // throw new OperationCanceledException($"The interceptor task '{GetType().FullName}' timed out.", ex);
            }
            finally
            {
                Unhook();
                cts.Cancel();
                cts.Dispose();
            }
        }

        protected virtual void Hook()
        {
            Dispatcher.Attach(this);
        }

        protected virtual void Unhook()
        {
            Dispatcher.Detach(this);
        }

        protected abstract Task OnExecuteAsync();

        protected virtual async Task<TResult> ExecuteAsync()
        {
            await OnExecuteAsync();
            return await completion.Task;
        }

        protected bool SetResult(TResult result) => completion.TrySetResult(result);
        protected bool SetCanceled() => completion.TrySetCanceled();
        protected bool SetException(Exception ex) => completion.TrySetException(ex);

        protected Task<bool> SendAsync(short header, params object[] values) => interceptor.SendToServerAsync(header, values);
        protected Task<bool> SendAsync(Packet packet) => interceptor.SendToServerAsync(packet);

        protected Task<bool> SendToClientAsync(short header, params object[] values) => interceptor.SendToClientAsync(header, values);
        protected Task<bool> SendToClientAsync(Packet packet) => interceptor.SendToClientAsync(packet);
    }
}
