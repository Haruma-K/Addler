#if !ADDLER_DISABLE_POOLING
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using Addler.Runtime.Core.LifetimeBinding;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace Addler.Runtime.Core.Pooling
{
    public sealed partial class AddressablePool : IDisposable
    {
        private readonly Dictionary<string, PooledObject> _busyObjects = new Dictionary<string, PooledObject>();
        private readonly Stack<GameObject> _usableObjects = new Stack<GameObject>();
        private bool _isWarmingUp;

        public AddressablePool(string key) : this(key, $"{key}Pool")
        {
        }

        public AddressablePool(object key, string poolName)
        {
            Key = key;
            Capacity = -1;
            Parent = new GameObject(poolName);
            Object.DontDestroyOnLoad(Parent);

            // If the parent GameObject is destroyed, the pool dispose itself.
            var releaseEvent = (IReleaseEvent)Parent.AddComponent<MonoBehaviourBasedReleaseEvent>();

            void OnDispatch()
            {
                if (!IsDisposed)
                    Dispose();

                releaseEvent.Dispatched -= OnDispatch;
            }

            releaseEvent.Dispatched += OnDispatch;
        }

        /// <summary>
        ///     GameObject that is the parent of the pooled GameObjects.
        /// </summary>
        public GameObject Parent { get; }

        public object Key { get; }

        public bool IsDisposed { get; private set; }

        /// <summary>
        ///     Number of objects managed by the pool.
        /// </summary>
        public int Capacity { get; private set; }

        /// <summary>
        ///     Number of the usable objects.
        /// </summary>
        public int UsableObjectsCount => _usableObjects.Count;

        public void Dispose()
        {
            if (IsDisposed)
                return;

            foreach (var handle in _busyObjects.Values)
                Addressables.ReleaseInstance(handle.Instance);

            foreach (var obj in _usableObjects)
                Addressables.ReleaseInstance(obj);

            Capacity = 0;
            _busyObjects.Clear();
            _usableObjects.Clear();

            if (Parent != null && !Parent.Equals(null))
                Object.Destroy(Parent);

            IsDisposed = true;
        }

        /// <summary>
        ///     Add or remove instances to the pool depending on capacity.
        /// </summary>
        /// <param name="capacity">Number of pre-generate objects.</param>
        /// <param name="progress"></param>
        public IEnumerator Warmup(int capacity, IProgress<float> progress = null)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().Name);

            if (capacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));

            if (_isWarmingUp)
                throw new InvalidOperationException(
                    $"This operation cannot be performed until the running {nameof(Warmup)} is complete.");

            _isWarmingUp = true;
            Capacity = capacity;
            var diffCount = capacity - _busyObjects.Count - _usableObjects.Count;
            if (diffCount >= 1)
            {
                // Generate new instances.
                var instantiateHandles = new List<AsyncOperationHandle>();
                for (var i = 0; i < diffCount; i++)
                {
                    var instantiateHandle = Addressables.InstantiateAsync(Key, Parent.transform);
                    instantiateHandles.Add(instantiateHandle);
                }

                var instantiateGroupHandle =
                    Addressables.ResourceManager.CreateGenericGroupOperation(instantiateHandles);
                while (!instantiateGroupHandle.IsDone)
                {
                    progress?.Report(instantiateGroupHandle.PercentComplete);
                    yield return null;
                }

                progress?.Report(1.0f);
                
                if (instantiateGroupHandle.Status == AsyncOperationStatus.Failed)
                    ExceptionDispatchInfo.Capture(instantiateGroupHandle.OperationException).Throw();

                foreach (var handle in instantiateGroupHandle.Result)
                {
                    var instance = handle.Convert<GameObject>().Result;
                    instance.SetActive(false);
                    _usableObjects.Push(instance);
                }
            }
            else if (diffCount <= -1)
            {
                // Remove unused objects
                for (var i = 0; i < -diffCount; i++)
                {
                    var obj = _usableObjects.Pop();
                    Addressables.ReleaseInstance(obj);
                }

                progress?.Report(1.0f);
            }

            _isWarmingUp = false;
        }

        /// <summary>
        ///     Use an instance in the object pool.
        /// </summary>
        /// <returns></returns>
        public PooledObject Use()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().Name);

            if (_usableObjects.Count == 0)
                throw new InvalidOperationException(
                    "There are no waiting objects available in ObjectPool. " +
                    $"You can expand the pool by calling {nameof(Warmup)}.");

            if (_isWarmingUp)
                throw new InvalidOperationException(
                    $"This operation cannot be performed until the running {nameof(Warmup)} is complete.");

            var instance = _usableObjects.Pop();

            // It seems that this instance has been destroyed outside the pool.
            if (instance == null)
                throw new InvalidOperationException(
                    "It seems that a GameObject you are trying to use has been destroyed outside the pool.");

            instance.SetActive(true);
            var handle = new PooledObject(this, instance);
            _busyObjects.Add(handle.Id, handle);
            return handle;
        }

        /// <summary>
        ///     Return a item to the object pool.
        /// </summary>
        /// <param name="obj"></param>
        public void Return(PooledObject obj)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().Name);

            var instance = obj.Instance;

            // If the returned instance has been destroyed outside the pool, do nothing.
            // InvalidOperationException will be thrown the next time this instance is retrieved from the pool.
            if (instance == null)
                return;

            instance.transform.SetParent(Parent.transform);
            instance.SetActive(false);

            _busyObjects.Remove(obj.Id);
            _usableObjects.Push(obj.Instance);
        }
    }
}
#endif
