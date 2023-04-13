using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Transversals.Business.Core.Domain.HostedServices
{
    public sealed class BackgroundTaskQueue
    {
        private readonly ConcurrentQueue<Func<IServiceProvider, Task>> _workItems = new ConcurrentQueue<Func<IServiceProvider, Task>>();
        private readonly SemaphoreSlim _signal = new SemaphoreSlim(0);

        public static BackgroundTaskQueue Instance { get; } = new BackgroundTaskQueue();

        /// <summary>Queues the background work item.</summary>
        /// <param name="data"></param>
        /// <exception cref="ArgumentNullException">workItem</exception>
        public void QueueBackgroundWorkItem(Func<IServiceProvider, Task> data)
        {
            _workItems.Enqueue(data);
            _signal.Release();
        }

        /// <summary>Dequeues the asynchronous.</summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public async Task<Func<IServiceProvider, Task>?> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _workItems.TryDequeue(out var workItem);

            return workItem;
        }

        public int Count => _workItems.Count;
    }
}
