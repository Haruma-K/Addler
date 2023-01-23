using System;
using System.Collections;
using Addler.Runtime.Core.LifetimeBinding;
using Development.Shared.Runtime.Scripts;
using Development.Tests.Runtime.Shared;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace Development.Tests.Runtime.LifetimeBinding
{
    public class AsyncOperationHandleExtensionsTest
    {
        [UnityTest]
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor)]
        public IEnumerator BindTo_BindToEventDispatcher()
        {
            var eventDispatcher = new AnonymousReleaseEvent();
            var referenceCounter = new ReferenceCounter(AssetPaths.SphereRedPlastic);
            var handle = Addressables.LoadAssetAsync<GameObject>(Addresses.SphereRedPlastic).BindTo(eventDispatcher);
            yield return handle;
            Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(1));
            eventDispatcher.Release();
            Assert.That(referenceCounter.ReferenceCount, Is.Zero);
        }

        [UnityTest]
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor)]
        public IEnumerator BindTo_BindToNullEventDispatcher_ArgumentNullException()
        {
            AnonymousReleaseEvent releaseEvent = null;
            var referenceCounter = new ReferenceCounter(AssetPaths.SphereRedPlastic);
            var handle = Addressables.LoadAssetAsync<GameObject>(Addresses.SphereRedPlastic);
            Assert.Throws<ArgumentNullException>(() => { handle.BindTo(releaseEvent); });
            yield return handle;
            Assert.That(referenceCounter.ReferenceCount, Is.Zero);
        }

        [UnityTest]
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor)]
        public IEnumerator BindTo_BindToGameObject()
        {
            var gameObject = new GameObject();
            var referenceCounter = new ReferenceCounter(AssetPaths.SphereRedPlastic);
            var handle = Addressables.LoadAssetAsync<GameObject>(Addresses.SphereRedPlastic).BindTo(gameObject);
            yield return handle;
            Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(1));
            Object.Destroy(gameObject);
            yield return null;
            Assert.That(referenceCounter.ReferenceCount, Is.Zero);
        }

        [UnityTest]
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor)]
        public IEnumerator BindTo_BindToNullGameObject_ArgumentNullException()
        {
            GameObject gameObject = null;
            var referenceCounter = new ReferenceCounter(AssetPaths.SphereRedPlastic);
            var handle = Addressables.LoadAssetAsync<GameObject>(Addresses.SphereRedPlastic);
            Assert.Throws<ArgumentNullException>(() => { handle.BindTo(gameObject); });
            yield return handle;
            Assert.That(referenceCounter.ReferenceCount, Is.Zero);
        }
    }
}
