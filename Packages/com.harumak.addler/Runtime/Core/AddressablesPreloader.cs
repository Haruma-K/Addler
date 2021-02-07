using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Addler.Runtime.Core
{
    /// <summary>
    ///     Preload the asset by Addressables.
    /// </summary>
    public class AddressablesPreloader : IDisposable
    {
        private readonly List<AsyncOperationHandle<object>> _handles = new List<AsyncOperationHandle<object>>();
        private readonly HashSet<object> _keys = new HashSet<object>();

        public bool IsDisposed { get; private set; }

        /// <summary>
        ///     Dispose the preloader and release all handles.
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            foreach (var handle in _handles)
            {
                Addressables.Release(handle);
            }

            IsDisposed = true;
        }

        /// <summary>
        ///     Get preloaded asset.
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public T Get<T>(object key)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            if (key.Equals(null))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (!_keys.Contains(key))
            {
                throw new InvalidOperationException(
                    $"{key} do not seem to be preloaded. Please call {nameof(PreloadAsync)} first.");
            }

            var handle = Addressables.LoadAssetAsync<object>(key);
            if (!handle.IsDone)
            {
                Addressables.Release(handle);
                throw new InvalidOperationException(
                    $"{key} do not seem to be preloaded. Please call {nameof(PreloadAsync)} first.");
            }

            if (handle.Status == AsyncOperationStatus.Failed)
            {
                Addressables.Release(handle);
                ExceptionDispatchInfo.Capture(handle.OperationException).Throw();
            }

            var obj = handle.Result;
            // Decrement the reference count immediately.
            Addressables.Release(handle);
            return (T) obj;
        }

        /// <summary>
        ///     Preload assets.
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public async Task PreloadAsync(params object[] keys)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            if (keys == null)
            {
                throw new ArgumentNullException(nameof(keys));
            }

            if (keys.Length.Equals(0))
            {
                throw new ArgumentException(nameof(keys));
            }

            var handles = keys.Select(Addressables.LoadAssetAsync<object>).ToArray();
            _handles.AddRange(handles);
            var tasks = handles.Select(handle => handle.Task);
            await Task.WhenAll(tasks);
            foreach (var handle in handles)
            {
                if (handle.Status == AsyncOperationStatus.Failed)
                {
                    ExceptionDispatchInfo.Capture(handle.OperationException).Throw();
                }
            }

            foreach (var key in keys)
            {
                _keys.Add(key);
            }
        }
    }
}