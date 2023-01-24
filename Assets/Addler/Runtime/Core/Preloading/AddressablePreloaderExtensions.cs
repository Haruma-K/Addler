#if !ADDLER_DISABLE_PRELOADING
using System;
using Addler.Runtime.Core.LifetimeBinding;
using UnityEngine;

namespace Addler.Runtime.Core.Preloading
{
    public static class AddressablePreloaderExtensions
    {
        /// <summary>
        ///     Binds the lifetime of the preloader to the <see cref="gameObject" />.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static AddressablePreloader BindTo(this AddressablePreloader self, GameObject gameObject)
        {
            if (gameObject == null)
            {
                self.Dispose();
                throw new ArgumentNullException(nameof(gameObject),
                    $"{nameof(gameObject)} is null so the preloader can't be bound and will be disposed immediately.");
            }

            if (!gameObject.TryGetComponent(out MonoBehaviourBasedReleaseEvent releaseEvent))
                releaseEvent = gameObject.AddComponent<MonoBehaviourBasedReleaseEvent>();

            return self.BindTo(releaseEvent);
        }

        /// <summary>
        ///     Binds the lifetime of the preloader to the <see cref="releaseEvent" />.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="releaseEvent"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static AddressablePreloader BindTo(this AddressablePreloader self, IReleaseEvent releaseEvent)
        {
            if (releaseEvent == null)
            {
                self.Dispose();
                throw new ArgumentNullException(nameof(releaseEvent),
                    $"{nameof(releaseEvent)} is null so the preloader can't be bound and will be disposed immediately.");
            }

            void OnRelease()
            {
                self.Dispose();
                releaseEvent.Dispatched -= OnRelease;
            }

            releaseEvent.Dispatched += OnRelease;
            return self;
        }
    }
}
#endif
