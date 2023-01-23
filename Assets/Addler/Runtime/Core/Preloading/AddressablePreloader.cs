#if !ADDLER_DISABLE_PRELOADING
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Addler.Runtime.Core.Preloading
{
    /// <summary>
    ///     Preloading system for Addressable Asset System.
    /// </summary>
    public sealed partial class AddressablePreloader : IDisposable
    {
        private readonly Dictionary<object, IReadOnlyList<AsyncOperationHandle>> _handles =
            new Dictionary<object, IReadOnlyList<AsyncOperationHandle>>();

        private readonly List<AsyncOperationHandle> _preloadHandles = new List<AsyncOperationHandle>();

        public bool IsDisposed { get; private set; }

        /// <summary>
        ///     Dispose the preloader and release all preloaded assets.
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed)
                return;

            foreach (var rootHandle in _preloadHandles)
                Addressables.Release(rootHandle);

            IsDisposed = true;
        }

        /// <summary>
        ///     Preload method for multiple keys (Addresses / Labels / AssetReferences).
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="progress"></param>
        /// <typeparam name="TObject"></typeparam>
        /// <returns></returns>
        public IEnumerator PreloadKeys<TObject>(IEnumerable<object> keys, IProgress<float> progress = null)
        {
            CheckDisposed();

            var preloadHandles = keys.Select(CreatePreloadHandle<TObject>).ToList();
            var groupOperation = Addressables.ResourceManager.CreateGenericGroupOperation(preloadHandles);
            _preloadHandles.Add(groupOperation);

            while (!groupOperation.IsDone)
            {
                progress?.Report(groupOperation.PercentComplete);
                yield return null;
            }

            progress?.Report(1.0f);

            if (groupOperation.Status == AsyncOperationStatus.Failed)
                ExceptionDispatchInfo.Capture(groupOperation.OperationException).Throw();
        }

        /// <summary>
        ///     Preload method for single key (Address / Label / AssetReference).
        /// </summary>
        /// <param name="key"></param>
        /// <param name="progress"></param>
        /// <typeparam name="TObject"></typeparam>
        /// <returns></returns>
        public IEnumerator PreloadKey<TObject>(object key, IProgress<float> progress = null)
        {
            CheckDisposed();

            var preloadHandle = CreatePreloadHandle<TObject>(key);
            _preloadHandles.Add(preloadHandle);

            while (!preloadHandle.IsDone)
            {
                progress?.Report(preloadHandle.PercentComplete);
                yield return null;
            }

            progress?.Report(1.0f);

            if (preloadHandle.Status == AsyncOperationStatus.Failed)
                ExceptionDispatchInfo.Capture(preloadHandle.OperationException).Throw();
        }

        private AsyncOperationHandle CreatePreloadHandle<TObject>(object key)
        {
            CheckDisposed();

            // If the key is already preloaded, return the completed handle.
            if (_handles.ContainsKey(key))
                return Addressables.ResourceManager.CreateCompletedOperation(default(TObject), null);

            var locationsHandle = Addressables.LoadResourceLocationsAsync(key, typeof(TObject));
            var chainedHandle = Addressables.ResourceManager.CreateChainOperation(locationsHandle, callback =>
            {
                var loadHandles = locationsHandle
                    .Result
                    .Select(Addressables.LoadAssetAsync<TObject>)
                    .Select(loadAssetHandle => (AsyncOperationHandle)loadAssetHandle)
                    .ToList();

                Addressables.Release(locationsHandle);
                _handles.Add(key, loadHandles);
                return Addressables.ResourceManager.CreateGenericGroupOperation(loadHandles);
            });

            return chainedHandle;
        }

        /// <summary>
        ///     Get preloaded asset.
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="TObject"></typeparam>
        /// <returns></returns>
        public TObject GetAsset<TObject>(object key)
        {
            CheckDisposed();

            if (!_handles.TryGetValue(key, out var handles))
                throw new InvalidOperationException(
                    $"The key {key} is not preloaded. Call {nameof(PreloadKey)}(s) first.");

            var handle = handles[0];
            if (!handle.IsDone)
                throw new InvalidOperationException($"Preloading of the key {key} is not completed yet.");

            return handle.Convert<TObject>().Result;
        }

        /// <summary>
        ///     Get preloaded assets.
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="TObject"></typeparam>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public IReadOnlyList<TObject> GetAssets<TObject>(object key)
        {
            CheckDisposed();

            if (!_handles.TryGetValue(key, out var handles))
                throw new InvalidOperationException(
                    $"The key {key} is not preloaded. Call {nameof(PreloadKey)}(s) first.");

            var result = new List<TObject>(handles.Count);
            foreach (var handle in handles)
            {
                if (!handle.IsDone)
                    throw new InvalidOperationException($"Preloading of the key {key} is not completed yet.");
                result.Add(handle.Convert<TObject>().Result);
            }

            return result;
        }

        /// <summary>
        ///     Release all preloaded assets.
        /// </summary>
        public void Clear()
        {
            CheckDisposed();

            foreach (var preloadHandles in _preloadHandles)
                Addressables.Release(preloadHandles);

            _preloadHandles.Clear();
            _handles.Clear();
        }

        private void CheckDisposed()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().Name);
        }
    }
}
#endif
