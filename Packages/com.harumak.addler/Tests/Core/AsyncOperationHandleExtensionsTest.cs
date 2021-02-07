using System;
using System.Collections;
using Addler.Runtime.Core;
using Addler.Runtime.Foundation.EventDispatcher;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace Addler.Tests.Core
{
    [PrebuildSetup(typeof(PrePostBuildProcess))]
    [PostBuildCleanup(typeof(PrePostBuildProcess))]
    public class AsyncOperationHandleExtensionsTest
    {
        [UnityTest]
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor)]
        public IEnumerator BindTo_BindToEventDispatcher()
        {
            var eventDispatcher = new AnonymousEventDispatcher();
            var referenceCounter = new ReferenceCounter(TestStrings.TestPrefabPath);
            var handle = Addressables.LoadAssetAsync<GameObject>(TestStrings.TestPrefabAddress).BindTo(eventDispatcher);
            yield return handle;
            Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(1));
            eventDispatcher.Dispatch();
            Assert.That(referenceCounter.ReferenceCount, Is.Zero);
        }

        [UnityTest]
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor)]
        public IEnumerator BindTo_BindToNullEventDispatcher_ArgumentNullException()
        {
            AnonymousEventDispatcher eventDispatcher = null;
            var referenceCounter = new ReferenceCounter(TestStrings.TestPrefabPath);
            var handle = Addressables.LoadAssetAsync<GameObject>(TestStrings.TestPrefabAddress);
            Assert.Throws<ArgumentNullException>(() => { handle.BindTo(eventDispatcher); });
            yield return handle;
            Assert.That(referenceCounter.ReferenceCount, Is.Zero);
        }

        [UnityTest]
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor)]
        public IEnumerator BindTo_BindToGameObject()
        {
            var gameObject = new GameObject();
            var referenceCounter = new ReferenceCounter(TestStrings.TestPrefabPath);
            var handle = Addressables.LoadAssetAsync<GameObject>(TestStrings.TestPrefabAddress).BindTo(gameObject);
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
            var referenceCounter = new ReferenceCounter(TestStrings.TestPrefabPath);
            var handle = Addressables.LoadAssetAsync<GameObject>(TestStrings.TestPrefabAddress);
            Assert.Throws<ArgumentNullException>(() => { handle.BindTo(gameObject); });
            yield return handle;
            Assert.That(referenceCounter.ReferenceCount, Is.Zero);
        }
    }
}