using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Transversals.Business.Core.Domain.HostedServices
{
    [ExcludeFromCodeCoverage]
    public sealed class BackgroundTaskList
    {
        private readonly ConcurrentBag<Func<IServiceProvider, Task>> _workItems = new ConcurrentBag<Func<IServiceProvider, Task>>();

        public static BackgroundTaskList Instance { get; } = new BackgroundTaskList();

        /// <summary>Queues the background work item.</summary>
        /// <param name="data"></param>
        /// <exception cref="ArgumentNullException">workItem</exception>
        public void Add(Func<IServiceProvider, Task> data)
        {
            _workItems.Add(data);

        }

        /// <summary>Dequeues the asynchronous.</summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public Func<IServiceProvider, Task>? Get(int index)
        {
            if (_workItems.Count > index)
            {
                return _workItems.ElementAt(index);
            }
            return null;
        }


        public int Count => _workItems.Count;
    }
}
