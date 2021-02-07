using System;
using Addler.Runtime.Foundation.EventDispatcher;
using UnityEngine;

namespace Addler.Runtime.Core
{
    public static class AddressablesPreloaderExtensions
    {
        /// <summary>
        ///     Binds the lifetime of the preloader to the <see cref="gameObject" />.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static AddressablesPreloader BindTo(this AddressablesPreloader self, GameObject gameObject)
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
        ///     Binds the lifetime of the preloader to the <see cref="eventDispatcher" />.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="eventDispatcher"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static AddressablesPreloader BindTo(this AddressablesPreloader self, IEventDispatcher eventDispatcher)
        {
            if (eventDispatcher == null)
            {
                self.Dispose();
                throw new ArgumentNullException(nameof(eventDispatcher));
            }

            void OnDispatch()
            {
                self.Dispose();
                eventDispatcher.OnDispatch -= OnDispatch;
            }

            eventDispatcher.OnDispatch += OnDispatch;
            return self;
        }
    }
}