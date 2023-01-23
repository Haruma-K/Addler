#if !ADDLER_DISABLE_POOLING
using System;
using System.Collections;
using Addler.Runtime.Core.Pooling;
using Development.Shared.Runtime.Scripts;
using Development.Tests.Runtime.Shared;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Development.Tests.Runtime.Pooling
{
    internal sealed class AddressablePoolTest
    {
        [UnityTest]
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor)]
        public IEnumerator WarmupAsync_Success()
        {
            var pool = new AddressablePool(Addresses.SphereRedPlastic);
            var referenceCounter = new ReferenceCounter(AssetPaths.SphereRedPlastic);
            yield return pool.Warmup(3);
            Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(3));
            pool.Dispose();
            Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(0));
        }

        [UnityTest]
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor)]
        public IEnumerator WarmupAsync_ShrinkPoolSize()
        {
            var pool = new AddressablePool(Addresses.SphereRedPlastic);
            var referenceCounter = new ReferenceCounter(AssetPaths.SphereRedPlastic);
            yield return pool.Warmup(3);
            Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(3));
            yield return pool.Warmup(1);
            yield return null;
            Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(1));
            pool.Dispose();
            Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(0));
        }

        [UnityTest]
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor)]
        public IEnumerator WarmupAsync_ExpandPoolSize()
        {
            var pool = new AddressablePool(Addresses.SphereRedPlastic);
            var referenceCounter = new ReferenceCounter(AssetPaths.SphereRedPlastic);
            yield return pool.Warmup(3);
            Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(3));
            yield return pool.Warmup(5);
            Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(5));
            pool.Dispose();
            Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(0));
        }

        [UnityTest]
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor)]
        public IEnumerator Use_CanGetInstance()
        {
            var pool = new AddressablePool(Addresses.SphereRedPlastic);
            var referenceCounter = new ReferenceCounter(AssetPaths.SphereRedPlastic);
            yield return pool.Warmup(1);
            Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(1));
            var instance = pool.Use().Instance;
            Assert.That(instance, Is.Not.Null);
            pool.Dispose();
            Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(0));
        }

        [UnityTest]
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor)]
        public IEnumerator Use_ExecuteTwice_CanGetAnotherInstance()
        {
            var pool = new AddressablePool(Addresses.SphereRedPlastic);
            var referenceCounter = new ReferenceCounter(AssetPaths.SphereRedPlastic);
            yield return pool.Warmup(2);
            Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(2));
            var instance1 = pool.Use().Instance;
            var instance2 = pool.Use().Instance;
            Assert.That(instance1, Is.Not.Null);
            Assert.That(instance2, Is.Not.Null);
            Assert.That(instance1, Is.Not.EqualTo(instance2));
            pool.Dispose();
            Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(0));
        }

        [UnityTest]
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor)]
        public IEnumerator Use_OverMaximumNumber_InvalidOperationException()
        {
            var pool = new AddressablePool(Addresses.SphereRedPlastic);
            var referenceCounter = new ReferenceCounter(AssetPaths.SphereRedPlastic);
            yield return pool.Warmup(1);
            Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(1));
            pool.Use();
            Assert.Throws<InvalidOperationException>(() => { pool.Use(); });
            pool.Dispose();
            Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(0));
        }

        [UnityTest]
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor)]
        public IEnumerator Use_InstanceIsUnderParent()
        {
            var pool = new AddressablePool(Addresses.SphereRedPlastic);
            var referenceCounter = new ReferenceCounter(AssetPaths.SphereRedPlastic);
            yield return pool.Warmup(1);
            Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(1));
            var instance = pool.Use().Instance;
            Assert.That(instance.transform.parent.gameObject, Is.EqualTo(pool.Parent));
            pool.Dispose();
            Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(0));
        }

        [UnityTest]
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor)]
        public IEnumerator DisposeOperation_CanReuse()
        {
            var pool = new AddressablePool(Addresses.SphereRedPlastic);
            var referenceCounter = new ReferenceCounter(AssetPaths.SphereRedPlastic);
            yield return pool.Warmup(1);
            Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(1));
            var operation1 = pool.Use();
            var instance1 = operation1.Instance;
            operation1.Dispose();
            var operation2 = pool.Use();
            var instance2 = operation2.Instance;
            operation2.Dispose();
            Assert.That(instance1, Is.EqualTo(instance2));
            pool.Dispose();
            Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(0));
        }

        [UnityTest]
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor)]
        public IEnumerator DisposeOperation_ObjectReturnsToPool()
        {
            var pool = new AddressablePool(Addresses.SphereRedPlastic);
            var referenceCounter = new ReferenceCounter(AssetPaths.SphereRedPlastic);
            yield return pool.Warmup(1);
            Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(1));
            var operation = pool.Use();
            var instance = operation.Instance;
            var tempObj = new GameObject();
            instance.transform.SetParent(tempObj.transform);
            operation.Dispose();
            Assert.That(instance.transform.parent.gameObject, Is.EqualTo(pool.Parent));
            pool.Dispose();
            Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(0));
        }
    }
}
#endif
