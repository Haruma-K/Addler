using System;
using Addler.Runtime.Foundation.EventDispatcher;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Addler.Runtime.Core
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
        public static AsyncOperationHandle BindTo(this AsyncOperationHandle self, GameObject gameObject)
        {
            if (gameObject.Equals(null))
            {
                Addressables.Release(self);
                throw new ArgumentNullException(nameof(gameObject));
            }

            if (!gameObject.TryGetComponent(out MonoBehaviourDestroyedEventDispatcher eventDispatcher))
            {
                eventDispatcher = gameObject.AddComponent<MonoBehaviourDestroyedEventDispatcher>();
            }

            return BindTo(self, eventDispatcher);
        }

        /// <summary>
        ///     Binds the lifetime of the handle to the <see cref="eventDispatcher" />.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="eventDispatcher"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static AsyncOperationHandle BindTo(this AsyncOperationHandle self, IEventDispatcher eventDispatcher)
        {
            if (eventDispatcher == null)
            {
                Addressables.Release(self);
                throw new ArgumentNullException(nameof(eventDispatcher));
            }

            void OnDispatch()
            {
                Addressables.Release(self);
                eventDispatcher.OnDispatch -= OnDispatch;
            }

            eventDispatcher.OnDispatch += OnDispatch;
            return self;
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
                Addressables.Release(self);
                throw new ArgumentNullException(nameof(gameObject));
            }

            ((AsyncOperationHandle) self).BindTo(gameObject);
            return self;
        }

        /// <summary>
        ///     Binds the lifetime of the handle to the <see cref="eventDispatcher" />.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="eventDispatcher"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static AsyncOperationHandle<T> BindTo<T>(this AsyncOperationHandle<T> self,
            IEventDispatcher eventDispatcher)
        {
            if (eventDispatcher == null)
            {
                Addressables.Release(self);
                throw new ArgumentNullException(nameof(eventDispatcher));
            }

            ((AsyncOperationHandle) self).BindTo(eventDispatcher);
            return self;
        }
    }
}