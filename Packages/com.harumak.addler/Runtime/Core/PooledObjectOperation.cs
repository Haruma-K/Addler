using System;
using Addler.Runtime.Foundation.ObjectPooling;

namespace Addler.Runtime.Core
{
    /// <summary>
    ///     Handles a GameObject that is managed by a pool.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PooledObjectOperation<T> : IDisposable
    {
        private readonly ObjectPoolHandle<T> _handle;
        private readonly Action<T> _onBeforeReturn;
        private readonly ObjectPool<T> _pool;

        public PooledObjectOperation(ObjectPool<T> pool, Action<T> onBeforeReturn = null)
        {
            _pool = pool;
            _handle = pool.Use();
            _onBeforeReturn = onBeforeReturn;
        }

        public bool IsDisposed { get; private set; }

        public T Object => _handle.Instance;

        /// <summary>
        ///     Dispose the handle and return the GameObject to the pool.
        /// </summary>
        public void Dispose()
        {
            // If the pool is destroyed, nothing will be done.
            // The instance should have already been destroyed.
            if (_pool.IsDisposed)
            {
                return;
            }

            if (IsDisposed)
            {
                return;
            }

            _onBeforeReturn?.Invoke(_handle.Instance);
            _pool.Return(_handle);
            IsDisposed = true;
        }
    }
}