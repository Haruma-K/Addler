using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Addler.Runtime.Core.LifetimeBinding
{
    public static class AsyncOperationHandleExtensions
    {
        /// <summary>
        ///     Binds the lifetime of the handle to the <see cref="gameObject" />.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static AsyncOperationHandle BindTo(this AsyncOperationHandle self, GameObject gameObject, bool isScene)
        {
            if (gameObject == null)
            {
                ReleaseHandle(self, isScene);
                throw new ArgumentNullException(nameof(gameObject),
                    $"{nameof(gameObject)} is null so the handle can't be bound and will be released immediately.");
            }

            if (!gameObject.TryGetComponent(out MonoBehaviourBasedReleaseEvent releaseEvent))
                releaseEvent = gameObject.AddComponent<MonoBehaviourBasedReleaseEvent>();

            return BindTo(self, releaseEvent, isScene);
        }

        /// <summary>
        ///     Binds the lifetime of the handle to the <see cref="gameObject" />.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="gameObject"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static AsyncOperationHandle<T> BindTo<T>(this AsyncOperationHandle<T> self, GameObject gameObject)
        {
            if (gameObject == null)
            {
                ReleaseHandle(self, typeof(T) == typeof(SceneInstance));
                throw new ArgumentNullException(nameof(gameObject),
                    $"{nameof(gameObject)} is null so the handle can't be bound and will be released immediately.");
            }

            ((AsyncOperationHandle)self).BindTo(gameObject, typeof(T) == typeof(SceneInstance));
            return self;
        }

        /// <summary>
        ///     Binds the lifetime of the handle to the <see cref="releaseEvent" />.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="releaseEvent"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static AsyncOperationHandle BindTo(this AsyncOperationHandle self, IReleaseEvent releaseEvent, bool isScene)
        {
            if (releaseEvent == null)
            {
                ReleaseHandle(self, isScene);
                throw new ArgumentNullException(nameof(releaseEvent),
                    $"{nameof(releaseEvent)} is null so the handle can't be bound and will be released immediately.");
            }

            void OnRelease()
            {
                ReleaseHandle(self, isScene);
                releaseEvent.Dispatched -= OnRelease;
            }

            releaseEvent.Dispatched += OnRelease;
            return self;
        }

        /// <summary>
        ///     Binds the lifetime of the handle to the <see cref="releaseEvent" />.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="releaseEvent"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static AsyncOperationHandle<T> BindTo<T>(this AsyncOperationHandle<T> self, IReleaseEvent releaseEvent)
        {
            if (releaseEvent == null)
            {
                ReleaseHandle(self, typeof(T) == typeof(SceneInstance));
                throw new ArgumentNullException(nameof(releaseEvent),
                    $"{nameof(releaseEvent)} is null so the handle can't be bound and will be released immediately.");
            }

            ((AsyncOperationHandle)self).BindTo(releaseEvent, typeof(T) == typeof(SceneInstance));
            return self;
        }

        private static void ReleaseHandle(AsyncOperationHandle handle, bool isScene)
        {
            if (isScene)
                Addressables.UnloadSceneAsync(handle);
            else
                Addressables.Release(handle);
        }
    }
}