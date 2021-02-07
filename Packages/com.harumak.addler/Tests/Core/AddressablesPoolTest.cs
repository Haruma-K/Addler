using System;
using System.Collections;
using Addler.Runtime.Core;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Addler.Tests.Core
{
    [PrebuildSetup(typeof(PrePostBuildProcess))]
    [PostBuildCleanup(typeof(PrePostBuildProcess))]
    public class AddressablesPoolTest
    {
        [UnityTest]
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor)]
        public IEnumerator WarmupAsync_Success()
        {
            var pool = new AddressablesPool(TestStrings.TestPrefabAddress);
            var referenceCounter = new ReferenceCounter(TestStrings.TestPrefabPath);
            yield return pool.WarmupAsync(3).AsIEnumerator();
            Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(3));
            pool.Dispose();
            Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(0));
        }

        [Test]
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor)]
        public void WarmupAsync_Disposed_ObjectDisposedException()
        {
            var pool = new AddressablesPool(TestStrings.TestPrefabAddress);
            pool.Dispose();
            Exception ex = null;
            try
            {
                pool.WarmupAsync(3).Wait();
            }
            catch (AggregateException e)
            {
                ex = e.InnerException;
                pool.Dispose();
            }

            Assert.That(ex, Is.TypeOf<ObjectDisposedException>());
        }

        [UnityTest]
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor)]
        public IEnumerator WarmupAsync_ShrinkPoolSize()
        {
            var pool = new AddressablesPool(TestStrings.TestPrefabAddress);
            var referenceCounter = new ReferenceCounter(TestStrings.TestPrefabPath);
            yield return pool.WarmupAsync(3).AsIEnumerator();
            Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(3));
            yield return pool.WarmupAsync(1).AsIEnumerator();
            yield return null;
            Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(1));
            pool.Dispose();
            Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(0));
        }

        [UnityTest]
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor)]
        public IEnumerator WarmupAsync_ExpandPoolSize()
        {
            var pool = new AddressablesPool(TestStrings.TestPrefabAddress);
            var referenceCounter = new ReferenceCounter(TestStrings.TestPrefabPath);
            yield return pool.WarmupAsync(3).AsIEnumerator();
            Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(3));
            yield return pool.WarmupAsync(5).AsIEnumerator();
            Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(5));
            pool.Dispose();
            Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(0));
        }

        [UnityTest]
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor)]
        public IEnumerator Use_CanGetInstance()
        {
            var pool = new AddressablesPool(TestStrings.TestPrefabAddress);
            var referenceCounter = new ReferenceCounter(TestStrings.TestPrefabPath);
            yield return pool.WarmupAsync(1).AsIEnumerator();
            Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(1));
            var instance = pool.Use().Object;
            Assert.That(instance, Is.Not.Null);
            pool.Dispose();
            Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(0));
        }

        [UnityTest]
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor)]
        public IEnumerator Use_ExecuteTwice_CanGetAnotherInstance()
        {
            var pool = new AddressablesPool(TestStrings.TestPrefabAddress);
            var referenceCounter = new ReferenceCounter(TestStrings.TestPrefabPath);
            yield return pool.WarmupAsync(2).AsIEnumerator();
            Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(2));
            var instance1 = pool.Use().Object;
            var instance2 = pool.Use().Object;
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
            var pool = new AddressablesPool(TestStrings.TestPrefabAddress);
            var referenceCounter = new ReferenceCounter(TestStrings.TestPrefabPath);
            yield return pool.WarmupAsync(1).AsIEnumerator();
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
            var pool = new AddressablesPool(TestStrings.TestPrefabAddress);
            var referenceCounter = new ReferenceCounter(TestStrings.TestPrefabPath);
            yield return pool.WarmupAsync(1).AsIEnumerator();
            Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(1));
            var instance = pool.Use().Object;
            Assert.That(instance.transform.parent.gameObject, Is.EqualTo(pool.PoolGameObject));
            pool.Dispose();
            Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(0));
        }

        [UnityTest]
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor)]
        public IEnumerator DisposeOperation_CanReuse()
        {
            var pool = new AddressablesPool(TestStrings.TestPrefabAddress);
            var referenceCounter = new ReferenceCounter(TestStrings.TestPrefabPath);
            yield return pool.WarmupAsync(1).AsIEnumerator();
            Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(1));
            var operation1 = pool.Use();
            var instance1 = operation1.Object;
            operation1.Dispose();
            var operation2 = pool.Use();
            var instance2 = operation2.Object;
            operation2.Dispose();
            Assert.That(instance1, Is.EqualTo(instance2));
            pool.Dispose();
            Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(0));
        }

        [UnityTest]
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor)]
        public IEnumerator DisposeOperation_ObjectReturnsToPool()
        {
            var pool = new AddressablesPool(TestStrings.TestPrefabAddress);
            var referenceCounter = new ReferenceCounter(TestStrings.TestPrefabPath);
            yield return pool.WarmupAsync(1).AsIEnumerator();
            Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(1));
            var operation = pool.Use();
            var instance = operation.Object;
            var tempObj = new GameObject();
            instance.transform.SetParent(tempObj.transform);
            operation.Dispose();
            Assert.That(instance.transform.parent.gameObject, Is.EqualTo(pool.PoolGameObject));
            pool.Dispose();
            Assert.That(referenceCounter.ReferenceCount, Is.EqualTo(0));
        }
    }
}