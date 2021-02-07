using System;
using Addler.Runtime.Foundation.EventDispatcher;
using UnityEngine;

namespace Addler.Runtime.Core
{
    public static class AddressablesPoolExtensions
    {
        /// <summary>
        ///     Binds the lifetime of the pool to the <see cref="GameObject" />.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static AddressablesPool BindTo(this AddressablesPool self, GameObject gameObject)
        {
            if (gameObject == null)
            {
                self.Dispose();
                throw new ArgumentNullException(nameof(gameObject));
            }

            if (!gameObject.TryGetComponent(out MonoBehaviourDestroyedEventDispatcher eventDispatcher))
            {
                eventDispatcher = gameObject.AddComponent<MonoBehaviourDestroyedEventDispatcher>();
            }

            return self.BindTo(eventDispatcher);
        }

        /// <summary>
        ///     Binds the lifetime of the pool to the <see cref="eventDispatcher" />.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="eventDispatcher"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static AddressablesPool BindTo(this AddressablesPool self, IEventDispatcher eventDispatcher)
        {
            if (eventDispatcher == null)
            {
                self.Dispose();
                throw new ArgumentNullException(nameof(eventDispatcher));
            }

            void OnDispatch()
            {
                if (!self.IsDisposed)
                {
                    self.Dispose();
                }

                eventDispatcher.OnDispatch -= OnDispatch;
            }

            eventDispatcher.OnDispatch += OnDispatch;
            return self;
        }

        /// <summary>
        ///     Binds the lifetime of the instance to the <see cref="gameObject" />.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static PooledObjectOperation<GameObject> BindTo(this PooledObjectOperation<GameObject> self,
            GameObject gameObject)
        {
            if (gameObject == null)
            {
                self.Dispose();
                throw new ArgumentNullException(nameof(gameObject));
            }

            if (!gameObject.TryGetComponent(out MonoBehaviourDestroyedEventDispatcher eventDispatcher))
            {
                eventDispatcher = gameObject.AddComponent<MonoBehaviourDestroyedEventDispatcher>();
            }

            return self.BindTo(eventDispatcher);
        }

        /// <summary>
        ///     Binds the lifetime of the instance to the <see cref="eventDispatcher" />.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="eventDispatcher"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static PooledObjectOperation<GameObject> BindTo(this PooledObjectOperation<GameObject> self,
            IEventDispatcher eventDispatcher)
        {
            if (eventDispatcher == null)
            {
                self.Dispose();
                throw new ArgumentNullException(nameof(eventDispatcher));
            }

            void OnDispatch()
            {
                if (!self.IsDisposed)
                {
                    self.Dispose();
                }

                eventDispatcher.OnDispatch -= OnDispatch;
            }

            eventDispatcher.OnDispatch += OnDispatch;
            return self;
        }
    }
}