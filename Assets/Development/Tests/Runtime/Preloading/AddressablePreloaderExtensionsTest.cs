#if !ADDLER_DISABLE_PRELOADING
using System;
using System.Collections;
using Addler.Runtime.Core.LifetimeBinding;
using Addler.Runtime.Core.Preloading;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace Development.Tests.Runtime.Preloading
{
    public class AddressablePreloaderExtensionsTest
    {
        [Test]
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor)]
        public void BindTo_BindToEventDispatcher()
        {
            var eventDispatcher = new AnonymousReleaseEvent();
            var preloader = new AddressablePreloader().BindTo(eventDispatcher);
            Assert.That(preloader.IsDisposed, Is.False);
            eventDispatcher.Release();
            Assert.That(preloader.IsDisposed, Is.True);
        }

        [Test]
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor)]
        public void BindTo_BindToNullEventDispatcher_ArgumentNullException()
        {
            AnonymousReleaseEvent releaseEvent = null;
            var preloader = new AddressablePreloader();
            Assert.Throws<ArgumentNullException>(() => { preloader.BindTo(releaseEvent); });
            Assert.That(preloader.IsDisposed, Is.True);
        }

        [UnityTest]
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor)]
        public IEnumerator BindTo_BindToGameObject()
        {
            var gameObject = new GameObject();
            var preloader = new AddressablePreloader().BindTo(gameObject);
            Assert.That(preloader.IsDisposed, Is.False);
            Object.Destroy(gameObject);
            yield return null;
            Assert.That(preloader.IsDisposed, Is.True);
        }

        [Test]
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor)]
        public void BindTo_BindToNullGameObject_ArgumentNullException()
        {
            GameObject gameObject = null;
            var preloader = new AddressablePreloader();
            Assert.Throws<ArgumentNullException>(() => { preloader.BindTo(gameObject); });
            Assert.That(preloader.IsDisposed, Is.True);
        }
    }
}
#endif
