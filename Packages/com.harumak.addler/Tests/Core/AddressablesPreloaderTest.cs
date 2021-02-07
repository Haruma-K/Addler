using System;
using System.Collections;
using Addler.Runtime.Core;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Addler.Tests.Core
{
    [PrebuildSetup(typeof(PrePostBuildProcess))]
    [PostBuildCleanup(typeof(PrePostBuildProcess))]
    public class AddressablesPreloaderTest
    {
        [UnityTest]
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor)]
        public IEnumerator PreloadAsync_CanPreload()
        {
            var preloader = new AddressablesPreloader();
            var referenceCounter = new ReferenceCounter(TestStrings.TestPrefabPath);
            yield return preloader.PreloadAsync(TestStrings.TestPrefabAddress).AsIEnumerator();
            Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(1));
            var prefab = preloader.Get<GameObject>(TestStrings.TestPrefabAddress);
            Assert.That(prefab, Is.EqualTo(AssetDatabase.LoadAssetAtPath<GameObject>(TestStrings.TestPrefabPath)));
            preloader.Dispose();

            // The handle is completely discarded in the next frame so wait a frame.
            yield return null;
            Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(0));
        }

        [UnityTest]
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor)]
        public IEnumerator PreloadAsync_Disposed_ObjectDisposedException()
        {
            var preloader = new AddressablesPreloader();
            var referenceCounter = new ReferenceCounter(TestStrings.TestPrefabPath);
            preloader.Dispose();
            Exception ex = null;
            try
            {
                preloader.PreloadAsync(TestStrings.TestPrefabAddress).Wait();
            }
            catch (AggregateException e)
            {
                ex = e.InnerException;
                preloader.Dispose();
            }

            Assert.That(ex, Is.TypeOf<ObjectDisposedException>());

            // The handle is completely discarded in the next frame so wait a frame.
            yield return null;
            Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(0));
        }

        [Test]
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor)]
        public void PreloadAsync_KeysIsNull_ArgumentNullException()
        {
            var preloader = new AddressablesPreloader();
            Exception ex = null;
            try
            {
                preloader.PreloadAsync(null).Wait();
            }
            catch (AggregateException e)
            {
                ex = e.InnerException;
                preloader.Dispose();
            }

            Assert.That(ex, Is.TypeOf<ArgumentNullException>());
        }

        [Test]
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor)]
        public void PreloadAsync_KeysIsEmpty_ArgumentException()
        {
            var preloader = new AddressablesPreloader();
            Exception ex = null;
            try
            {
                preloader.PreloadAsync().Wait();
            }
            catch (AggregateException e)
            {
                ex = e.InnerException;
                preloader.Dispose();
            }

            Assert.That(ex, Is.TypeOf<ArgumentException>());
        }

        [UnityTest]
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor)]
        public IEnumerator Get_CanGet()
        {
            var preloader = new AddressablesPreloader();
            var referenceCounter = new ReferenceCounter(TestStrings.TestPrefabPath);
            yield return preloader.PreloadAsync(TestStrings.TestPrefabAddress).AsIEnumerator();
            Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(1));
            var prefab = preloader.Get<GameObject>(TestStrings.TestPrefabAddress);
            Assert.That(prefab, Is.EqualTo(AssetDatabase.LoadAssetAtPath<GameObject>(TestStrings.TestPrefabPath)));
            preloader.Dispose();

            // The handle is completely discarded in the next frame so wait a frame.
            yield return null;
            Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(0));
        }

        [UnityTest]
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor)]
        public IEnumerator Get_Disposed_ObjectDisposedException()
        {
            var preloader = new AddressablesPreloader();
            var referenceCounter = new ReferenceCounter(TestStrings.TestPrefabPath);
            yield return preloader.PreloadAsync(TestStrings.TestPrefabAddress).AsIEnumerator();
            Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(1));
            preloader.Dispose();
            Assert.Throws<ObjectDisposedException>(() => { preloader.Get<GameObject>(TestStrings.TestPrefabAddress); });

            // The handle is completely discarded in the next frame so wait a frame.
            yield return null;
            Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(0));
        }

        [UnityTest]
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor)]
        public IEnumerator Get_NotPreloaded_InvalidOperationException()
        {
            var preloader = new AddressablesPreloader();
            var referenceCounter = new ReferenceCounter(TestStrings.TestPrefabPath);

            Assert.Throws<InvalidOperationException>(() =>
            {
                try
                {
                    preloader.Get<GameObject>(TestStrings.TestPrefabAddress);
                }
                finally
                {
                    preloader.Dispose();
                }
            });

            // The handle is completely discarded in the next frame so wait a frame.
            yield return null;
            Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(0));
        }
    }
}