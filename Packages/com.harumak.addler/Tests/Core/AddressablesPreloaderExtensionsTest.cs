using System;
using System.Collections;
using Addler.Runtime.Core;
using Addler.Runtime.Foundation.EventDispatcher;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace Addler.Tests.Core
{
    [PrebuildSetup(typeof(PrePostBuildProcess))]
    [PostBuildCleanup(typeof(PrePostBuildProcess))]
    public class AddressablesPreloaderExtensionsTest
    {
        [Test]
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor)]
        public void BindTo_BindToEventDispatcher()
        {
            var eventDispatcher = new AnonymousEventDispatcher();
            var preloader = new AddressablesPreloader().BindTo(eventDispatcher);
            Assert.That(preloader.IsDisposed, Is.False);
            eventDispatcher.Dispatch();
            Assert.That(preloader.IsDisposed, Is.True);
        }

        [Test]
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor)]
        public void BindTo_BindToNullEventDispatcher_ArgumentNullException()
        {
            AnonymousEventDispatcher eventDispatcher = null;
            var preloader = new AddressablesPreloader();
            Assert.Throws<ArgumentNullException>(() => { preloader.BindTo(eventDispatcher); });
            Assert.That(preloader.IsDisposed, Is.True);
        }

        [UnityTest]
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor)]
        public IEnumerator BindTo_BindToGameObject()
        {
            var gameObject = new GameObject();
            var preloader = new AddressablesPreloader().BindTo(gameObject);
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
            var preloader = new AddressablesPreloader();
            Assert.Throws<ArgumentNullException>(() => { preloader.BindTo(gameObject); });
            Assert.That(preloader.IsDisposed, Is.True);
        }
    }
}