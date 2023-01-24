#if !ADDLER_DISABLE_PRELOADING
using System;
using System.Collections;
using System.Linq;
using Addler.Runtime.Core.Preloading;
using Development.Shared.Runtime.Scripts;
using Development.Tests.Runtime.Shared;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AddressableAssets;
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
            var expectedPrefabHandle = Addressables.LoadAssetAsync<GameObject>(address);
            yield return expectedPrefabHandle;
            Assert.That(prefab, Is.EqualTo(expectedPrefabHandle.Result));

            Addressables.Release(expectedPrefabHandle);
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
            var expectedPrefabHandle1 = Addressables.LoadAssetAsync<GameObject>(address1);
            var expectedPrefabHandle2 = Addressables.LoadAssetAsync<GameObject>(address2);
            yield return expectedPrefabHandle1;
            yield return expectedPrefabHandle2;

            var prefab1 = preloader.GetAsset<GameObject>(address1);
            Assert.That(prefab1, Is.EqualTo(expectedPrefabHandle1.Result));
            var prefab2 = preloader.GetAsset<GameObject>(address2);
            Assert.That(prefab2, Is.EqualTo(expectedPrefabHandle2.Result));

            Addressables.Release(expectedPrefabHandle1);
            Addressables.Release(expectedPrefabHandle2);
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
            var expectedPrefabsHandle = Addressables.LoadAssetsAsync<GameObject>(label, null);
            yield return expectedPrefabsHandle;
            var expectedPrefabs = expectedPrefabsHandle.Result;

            foreach (var prefab in prefabs)
                Assert.That(expectedPrefabs.Contains(prefab), Is.True);

            Addressables.Release(expectedPrefabsHandle);
            preloader.Dispose();

            // The handle is completely discarded in the next frame so wait a frame.
            yield return null;
            foreach (var referenceCounter in referenceCounters)
                Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(0));
        }
    }
}
#endif
