using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Addler.Runtime.Foundation.EventDispatcher;
using Addler.Runtime.Foundation.ObjectPooling;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace Addler.Runtime.Core
{
    /// <summary>
    ///     Load the prefab by Addressables and pool the instance.
    /// </summary>
    public class AddressablesPool
    {
        private readonly ObjectPool<GameObject> _pool;

        public AddressablesPool(string key) : this(key, $"{key}Pool")
        {
        }

        public AddressablesPool(object key, string poolName)
        {
            _pool = new ObjectPool<GameObject>(() => CreateInstanceAsync(key), ReleaseInstance);
            var parentObj = new GameObject(poolName);
            PoolGameObject = parentObj;
            Object.DontDestroyOnLoad(PoolGameObject.gameObject);

            if (!parentObj.TryGetComponent<MonoBehaviourDestroyedEventDispatcher>(out var disposedEventDispatcher))
            {
                disposedEventDispatcher = parentObj.AddComponent<MonoBehaviourDestroyedEventDispatcher>();
            }

            // If the parent GameObject is destroyed, the pool dispose itself.
            void OnDispatch()
            {
                if (!IsDisposed)
                {
                    Dispose();
                }

                disposedEventDispatcher.OnDispatch -= OnDispatch;
            }

            disposedEventDispatcher.OnDispatch += OnDispatch;
        }

        /// <summary>
        ///     GameObject that is the parent of the pooled GameObjects.
        /// </summary>
        public GameObject PoolGameObject { get; }

        public bool IsDisposed { get; private set; }

        /// <summary>
        ///     Load prefab and instantiate GameObjects for the amount of <see cref="capacity" />.
        /// </summary>
        /// <param name="capacity"></param>
        /// <returns></returns>
        /// <exception cref="ObjectDisposedException"></exception>
        public async Task WarmupAsync(int capacity)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            await _pool.WarmupAsync(capacity);
        }

        /// <summary>
        ///     Use a pooled GameObject.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public PooledObjectOperation<GameObject> Use()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            var operation = new PooledObjectOperation<GameObject>(_pool,
                x =>
                {
                    // If the returned instance has been destroyed outside the pool, do nothing.
                    // InvalidOperationException will be thrown the next time this instance is retrieved from the pool.
                    if (x.Equals(null))
                    {
                        return;
                    }

                    x.transform.SetParent(PoolGameObject.transform);
                    x.SetActive(false);
                });

            var instance = operation.Object;

            // It seems that this instance has been destroyed outside the pool.
            if (instance.Equals(null))
            {
                throw new InvalidOperationException(
                    "It seems that a GameObject you are trying to use has been destroyed outside the pool.");
            }

            instance.SetActive(true);

            return operation;
        }

        /// <summary>
        ///     Dispose the pool and all managed GameObjects.
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            _pool.Dispose();
            if (PoolGameObject != null && !PoolGameObject.Equals(null))
            {
                Object.Destroy(PoolGameObject.gameObject);
            }

            IsDisposed = true;
        }

        private async Task<GameObject> CreateInstanceAsync(object key)
        {
            var handle = Addressables.InstantiateAsync(key, PoolGameObject.transform);
            await handle.Task;
            if (handle.Status == AsyncOperationStatus.Failed)
            {
                ExceptionDispatchInfo.Capture(handle.OperationException).Throw();
            }

            var instance = handle.Result;
            instance.gameObject.SetActive(false);
            return instance;
        }

        private void ReleaseInstance(GameObject obj)
        {
            // If the instance is being destroyed outside of the pool's control, do nothing.
            // Addressables reference counter should have already been decremented.
            if (obj.Equals(null))
            {
                return;
            }

            Addressables.ReleaseInstance(obj);
        }
    }
}