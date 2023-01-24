#if !ADDLER_DISABLE_POOLING
using System;
using Addler.Runtime.Core.LifetimeBinding;
using UnityEngine;

namespace Addler.Runtime.Core.Pooling
{
    public static class AddressablePoolExtensions
    {
        /// <summary>
        ///     Binds the lifetime of the pool to the <see cref="GameObject" />.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static AddressablePool BindTo(this AddressablePool self, GameObject gameObject)
        {
            if (gameObject == null)
            {
                self.Dispose();
                throw new ArgumentNullException(nameof(gameObject));
            }

            if (!gameObject.TryGetComponent(out MonoBehaviourBasedReleaseEvent eventDispatcher))
                eventDispatcher = gameObject.AddComponent<MonoBehaviourBasedReleaseEvent>();

            return self.BindTo(eventDispatcher);
        }

        /// <summary>
        ///     Binds the lifetime of the pool to the <see cref="releaseEvent" />.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="releaseEvent"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static AddressablePool BindTo(this AddressablePool self, IReleaseEvent releaseEvent)
        {
            if (releaseEvent == null)
            {
                self.Dispose();
                throw new ArgumentNullException(nameof(releaseEvent));
            }

            void OnDispatch()
            {
                if (!self.IsDisposed)
                    self.Dispose();

                releaseEvent.Dispatched -= OnDispatch;
            }

            releaseEvent.Dispatched += OnDispatch;
            return self;
        }

        /// <summary>
        ///     Binds the lifetime of the instance to the <see cref="gameObject" />.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static PooledObject BindTo(this PooledObject self,
            GameObject gameObject)
        {
            if (gameObject == null)
            {
                self.Dispose();
                throw new ArgumentNullException(nameof(gameObject));
            }

            if (!gameObject.TryGetComponent(out MonoBehaviourBasedReleaseEvent eventDispatcher))
                eventDispatcher = gameObject.AddComponent<MonoBehaviourBasedReleaseEvent>();

            return self.BindTo(eventDispatcher);
        }

        /// <summary>
        ///     Binds the lifetime of the instance to the <see cref="releaseEvent" />.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="releaseEvent"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static PooledObject BindTo(this PooledObject self,
            IReleaseEvent releaseEvent)
        {
            if (releaseEvent == null)
            {
                self.Dispose();
                throw new ArgumentNullException(nameof(releaseEvent));
            }

            void OnDispatch()
            {
                if (!self.IsDisposed)
                    self.Dispose();

                releaseEvent.Dispatched -= OnDispatch;
            }

            releaseEvent.Dispatched += OnDispatch;
            return self;
        }
    }
}
#endif
