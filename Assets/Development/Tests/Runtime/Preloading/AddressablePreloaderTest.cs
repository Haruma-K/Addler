#if !ADDLER_DISABLE_PRELOADING
using System;
using System.Collections;
using System.Linq;
using Addler.Runtime.Core.Preloading;
using Development.Shared.Runtime.Scripts;
using Development.Tests.Runtime.Shared;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Development.Tests.Runtime.Preloading
{
    internal sealed class AddressablePreloaderTest
    {
        [UnityTest]
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor)]
        public IEnumerator PreloadAddress()
        {
            const string address = Addresses.SphereRedPlastic;
            const string assetPath = AssetPaths.SphereRedPlastic;
            var preloader = new AddressablePreloader();
            var referenceCounter = new ReferenceCounter(assetPath);
            yield return preloader.PreloadKey<GameObject>(address);
            Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(1));
            var prefab = preloader.GetAsset<GameObject>(address);
            Assert.That(prefab, Is.EqualTo(AssetDatabase.LoadAssetAtPath<GameObject>(assetPath)));

            preloader.Dispose();

            // The handle is completely discarded in the next frame so wait a frame.
            yield return null;
            Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(0));
        }

        [Test]
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor)]
        public void PreloadAddress_NotPreloaded_InvalidOperationException()
        {
            const string address = Addresses.SphereRedPlastic;
            var preloader = new AddressablePreloader();

            Assert.That(() => preloader.GetAsset<GameObject>(address), Throws.TypeOf<InvalidOperationException>());

            preloader.Dispose();
        }

        [UnityTest]
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor)]
        public IEnumerator PreloadAddresses()
        {
            const string address1 = Addresses.SphereRedPlastic;
            const string address2 = Addresses.SphereBluePlastic;
            var addresses = new[] { address1, address2 };
            const string assetPath1 = AssetPaths.SphereRedPlastic;
            const string assetPath2 = AssetPaths.SphereBluePlastic;

            var preloader = new AddressablePreloader();
            var referenceCounter1 = new ReferenceCounter(assetPath1);
            var referenceCounter2 = new ReferenceCounter(assetPath2);
            yield return preloader.PreloadKeys<GameObject>(addresses);
            Assert.That(referenceCounter1.ReferenceCount, Is.EqualTo(1));
            Assert.That(referenceCounter2.ReferenceCount, Is.EqualTo(1));

            var prefab1 = preloader.GetAsset<GameObject>(address1);
            Assert.That(prefab1, Is.EqualTo(AssetDatabase.LoadAssetAtPath<GameObject>(assetPath1)));
            var prefab2 = preloader.GetAsset<GameObject>(address2);
            Assert.That(prefab2, Is.EqualTo(AssetDatabase.LoadAssetAtPath<GameObject>(assetPath2)));

            preloader.Dispose();

            // The handle is completely discarded in the next frame so wait a frame.
            yield return null;
            Assert.That(referenceCounter1.ReferenceCount, Is.EqualTo(0));
            Assert.That(referenceCounter2.ReferenceCount, Is.EqualTo(0));
        }

        [UnityTest]
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor)]
        public IEnumerator PreloadLabel()
        {
            const string label = LabelNames.Plastic;
            var assetPaths = AssetPaths.EnumerateSpherePlastic().ToArray();
            var preloader = new AddressablePreloader();
            var referenceCounters = assetPaths.Select(x => new ReferenceCounter(x)).ToArray();
            yield return preloader.PreloadKey<GameObject>(label);
            foreach (var referenceCounter in referenceCounters)
                Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(1));

            var prefabs = preloader.GetAssets<GameObject>(label);
            foreach (var assetPath in assetPaths)
            {
                var expected = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                Assert.That(prefabs.Contains(expected), Is.True);
            }

            preloader.Dispose();

            // The handle is completely discarded in the next frame so wait a frame.
            yield return null;
            foreach (var referenceCounter in referenceCounters)
                Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(0));
        }
    }
}
#endif
