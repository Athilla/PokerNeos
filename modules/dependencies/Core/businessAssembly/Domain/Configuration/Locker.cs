using System;
using System.Threading;

namespace Transversals.Business.Core.Domain.Configuration
{
    public static class Locker
    {
        private static readonly SemaphoreSlim toLock = new SemaphoreSlim(1, 1);

        public static LockReleaser Lock()
        {
            toLock.Wait();
            return new LockReleaser(toLock);
        }

        public struct LockReleaser : IDisposable
        {
            private readonly SemaphoreSlim toRelease;

            public LockReleaser(SemaphoreSlim toRelease)
            {
                this.toRelease = toRelease;
            }
            public void Dispose()
            {
                toRelease.Release();
            }
        }
    }
}
