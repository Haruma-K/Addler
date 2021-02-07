using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Addler.Runtime.Foundation.ObjectPooling
{
    /// <summary>
    ///     Object pool that creates objects asynchronously.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ObjectPool<T> : IDisposable
    {
        private readonly Dictionary<int, T> _busyObjects;
        private readonly Action<T> _disposeAction;
        private readonly Func<Task<T>> _factoryFunc;
        private readonly Stack<T> _waitingObjects;
        private readonly object _waitingObjectsLocker = new object();
        private int _currentId;
        private bool _isGenerating;

        public ObjectPool(Func<Task<T>> factoryFunc, Action<T> disposeAction = null)
        {
            if (factoryFunc == null)
            {
                throw new ArgumentNullException(nameof(factoryFunc));
            }

            Capacity = -1;
            _factoryFunc = factoryFunc;
            _disposeAction = disposeAction;
            _waitingObjects = new Stack<T>();
            _busyObjects = new Dictionary<int, T>();
        }

        public bool IsDisposed { get; private set; }

        /// <summary>
        ///     Number of objects managed by the pool.
        /// </summary>
        public int Capacity { get; private set; }

        /// <summary>
        ///     Number of the waiting objects.
        /// </summary>
        public int WaitingObjectsCount => _waitingObjects.Count;

        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            foreach (var obj in _busyObjects)
            {
                _disposeAction?.Invoke(obj.Value);
            }

            foreach (var obj in _waitingObjects)
            {
                _disposeAction?.Invoke(obj);
            }

            Capacity = 0;
            _busyObjects.Clear();
            _waitingObjects.Clear();

            IsDisposed = true;
        }

        /// <summary>
        ///     Add or remove instances to the pool depending on capacity.
        /// </summary>
        /// <param name="capacity">Number of pre-generate objects.</param>
        public async Task WarmupAsync(int capacity)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            if (capacity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity));
            }

            if (_isGenerating)
            {
                throw new InvalidOperationException(
                    $"This operation cannot be performed until the running {nameof(WarmupAsync)} is complete.");
            }

            _isGenerating = true;
            Capacity = capacity;
            var count = capacity - _busyObjects.Count - _waitingObjects.Count;
            if (count >= 1)
            {
                await AddWaitingObjectsAsync(count).ConfigureAwait(false);
            }
            else if (count <= -1)
            {
                RemoveWaitingObject(-count);
            }

            _isGenerating = false;
        }

        /// <summary>
        ///     Use an instance in the object pool.
        /// </summary>
        /// <returns></returns>
        public ObjectPoolHandle<T> Use()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            if (_waitingObjects.Count == 0)
            {
                throw new InvalidOperationException(
                    "There are no waiting objects available in ObjectPool. " +
                    $"You can expand the pool by calling {nameof(WarmupAsync)}.");
            }

            if (_isGenerating)
            {
                throw new InvalidOperationException(
                    $"This operation cannot be performed until the running {nameof(WarmupAsync)} is complete.");
            }

            var obj = _waitingObjects.Pop();
            var id = _currentId++;
            _busyObjects.Add(id, obj);
            return new ObjectPoolHandle<T>(id, obj);
        }

        /// <summary>
        ///     Return a item to the object pool.
        /// </summary>
        /// <param name="handle"></param>
        public void Return(ObjectPoolHandle<T> handle)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            if (!_busyObjects.ContainsKey(handle.Id))
            {
                throw new InvalidOperationException($"The handle ID {handle.Id} is invalid.");
            }

            _busyObjects.Remove(handle.Id);
            _waitingObjects.Push(handle.Instance);
        }

        private async Task AddWaitingObjectsAsync(int count)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            var tasks = new Task[count];
            for (var i = 0; i < count; i++)
            {
                tasks[i] = AddWaitingObjectAsync();
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        private async Task AddWaitingObjectAsync()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            var obj = await _factoryFunc().ConfigureAwait(false);

            if (IsDisposed)
            {
                _disposeAction?.Invoke(obj);
                throw new ObjectDisposedException(GetType().Name);
            }

            lock (_waitingObjectsLocker)
            {
                _waitingObjects.Push(obj);
            }
        }

        private void RemoveWaitingObject(int count)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            if (count > _waitingObjects.Count)
            {
                throw new InvalidOperationException(
                    "Tried to remove an instance from the pool, but there are not enough waiting objects.");
            }

            for (var i = 0; i < count; i++)
            {
                var obj = _waitingObjects.Pop();
                _disposeAction?.Invoke(obj);
            }
        }
    }
}