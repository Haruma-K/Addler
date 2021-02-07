using System;
using System.Threading;
using System.Threading.Tasks;
using Addler.Runtime.Foundation.ObjectPooling;
using NUnit.Framework;

namespace Addler.Tests.Foundation
{
    public class ObjectPoolingTest
    {
        [Test]
        public void WarmupAsync_Success()
        {
            var pool = new ObjectPool<DummyPooledObj>(() => CreateDummyPooledObjAsync(10));
            pool.WarmupAsync(3).Wait();
            Assert.That(pool.Capacity, Is.EqualTo(3));
            Assert.That(pool.WaitingObjectsCount, Is.EqualTo(3));
            pool.Dispose();
        }

        [Test]
        public void WarmupAsync_Expand_Success()
        {
            var pool = new ObjectPool<DummyPooledObj>(() => CreateDummyPooledObjAsync(10));
            pool.WarmupAsync(1).Wait();
            Assert.That(pool.Capacity, Is.EqualTo(1));
            Assert.That(pool.WaitingObjectsCount, Is.EqualTo(1));
            pool.WarmupAsync(2).Wait();
            Assert.That(pool.Capacity, Is.EqualTo(2));
            Assert.That(pool.WaitingObjectsCount, Is.EqualTo(2));
            pool.Dispose();
        }

        [Test]
        public void WarmupAsync_Shrink_Success()
        {
            var pool = new ObjectPool<DummyPooledObj>(() => CreateDummyPooledObjAsync(10));
            pool.WarmupAsync(2).Wait();
            Assert.That(pool.Capacity, Is.EqualTo(2));
            Assert.That(pool.WaitingObjectsCount, Is.EqualTo(2));
            pool.WarmupAsync(1).Wait();
            Assert.That(pool.Capacity, Is.EqualTo(1));
            Assert.That(pool.WaitingObjectsCount, Is.EqualTo(1));
            pool.Dispose();
        }

        [Test]
        public void WarmupAsync_Disposed_ObjectDisposedException()
        {
            var pool = new ObjectPool<DummyPooledObj>(() => CreateDummyPooledObjAsync(10));
            pool.Dispose();

            AggregateException ex = null;
            try
            {
                pool.WarmupAsync(1).Wait();
            }
            catch (AggregateException e)
            {
                ex = e;
            }

            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.InnerExceptions.Count, Is.EqualTo(1));
            Assert.That(ex.InnerExceptions[0], Is.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void WarmupAsync_CapacityIsLessThanZero_ArgumentOutOfRangeException()
        {
            var pool = new ObjectPool<DummyPooledObj>(() => CreateDummyPooledObjAsync(10));
            AggregateException ex = null;
            try
            {
                pool.WarmupAsync(-1).Wait();
            }
            catch (AggregateException e)
            {
                ex = e;
            }

            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.InnerExceptions.Count, Is.EqualTo(1));
            Assert.That(ex.InnerExceptions[0], Is.TypeOf<ArgumentOutOfRangeException>());
            pool.Dispose();
        }

        [Test]
        public void WarmupAsync_AlreadyProcessing_InvalidOperationException()
        {
            var pool = new ObjectPool<DummyPooledObj>(() => CreateDummyPooledObjAsync(1000));
            pool.WarmupAsync(1);
            AggregateException ex = null;
            try
            {
                pool.WarmupAsync(1).Wait();
            }
            catch (AggregateException e)
            {
                ex = e;
            }

            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.InnerExceptions.Count, Is.EqualTo(1));
            Assert.That(ex.InnerExceptions[0], Is.TypeOf<InvalidOperationException>());
            pool.Dispose();
        }

        [Test]
        public void Use_Success()
        {
            var pool = new ObjectPool<DummyPooledObj>(() => CreateDummyPooledObjAsync(10));
            pool.WarmupAsync(1).Wait();
            var handle = pool.Use();
            Assert.That(handle.Instance, Is.TypeOf<DummyPooledObj>());
            Assert.That(pool.Capacity, Is.EqualTo(1));
            Assert.That(pool.WaitingObjectsCount, Is.EqualTo(0));
            pool.Dispose();
        }

        [Test]
        public void UseContinuously_Success()
        {
            var pool = new ObjectPool<DummyPooledObj>(() => CreateDummyPooledObjAsync(10));
            pool.WarmupAsync(2).Wait();
            Assert.That(pool.WaitingObjectsCount, Is.EqualTo(2));
            var handle1 = pool.Use();
            Assert.That(pool.WaitingObjectsCount, Is.EqualTo(1));
            var handle2 = pool.Use();
            Assert.That(pool.WaitingObjectsCount, Is.EqualTo(0));
            Assert.That(handle1.Instance, Is.TypeOf<DummyPooledObj>());
            Assert.That(handle2.Instance, Is.TypeOf<DummyPooledObj>());
            Assert.That(handle1.Instance, Is.Not.EqualTo(handle2.Instance));
            Assert.That(pool.Capacity, Is.EqualTo(2));
            pool.Dispose();
        }

        [Test]
        public void Use_Reuse_Success()
        {
            var pool = new ObjectPool<DummyPooledObj>(() => CreateDummyPooledObjAsync(10));
            pool.WarmupAsync(1).Wait();
            var handle1 = pool.Use();
            pool.Return(handle1);
            var handle2 = pool.Use();
            Assert.That(handle1.Instance, Is.EqualTo(handle2.Instance));
            pool.Dispose();
        }

        [Test]
        public void Use_Disposed_ObjectDisposedException()
        {
            var pool = new ObjectPool<DummyPooledObj>(() => CreateDummyPooledObjAsync(10));
            pool.WarmupAsync(1).Wait();
            pool.Dispose();
            Assert.Throws<ObjectDisposedException>(() => { pool.Use(); });
        }

        [Test]
        public void Use_WaitingObjectCountIsZero_InvalidOperationException()
        {
            var pool = new ObjectPool<DummyPooledObj>(() => CreateDummyPooledObjAsync(10));
            pool.WarmupAsync(1).Wait();
            pool.Use();
            Assert.Throws<InvalidOperationException>(() => { pool.Use(); });
            pool.Dispose();
        }

        [Test]
        public void Use_WarmupIsNotCompleted_InvalidOperationException()
        {
            var pool = new ObjectPool<DummyPooledObj>(() => CreateDummyPooledObjAsync(1000));
            pool.WarmupAsync(1);
            Assert.Throws<InvalidOperationException>(() => { pool.Use(); });
            pool.Dispose();
        }

        [Test]
        public void Return_Success()
        {
            var pool = new ObjectPool<DummyPooledObj>(() => CreateDummyPooledObjAsync(10));
            pool.WarmupAsync(1).Wait();
            var handle = pool.Use();
            Assert.That(pool.WaitingObjectsCount, Is.EqualTo(0));
            pool.Return(handle);
            Assert.That(pool.WaitingObjectsCount, Is.EqualTo(1));
            pool.Dispose();
        }

        [Test]
        public void Return_Disposed_ObjectDisposedException()
        {
            var pool = new ObjectPool<DummyPooledObj>(() => CreateDummyPooledObjAsync(10));
            pool.WarmupAsync(1).Wait();
            var handle = pool.Use();
            pool.Dispose();
            Assert.Throws<ObjectDisposedException>(() => { pool.Return(handle); });
        }

        [Test]
        public void Return_ReturnReturnedObject_InvalidOperationException()
        {
            var pool = new ObjectPool<DummyPooledObj>(() => CreateDummyPooledObjAsync(10));
            pool.WarmupAsync(1).Wait();
            var handle = pool.Use();
            pool.Return(handle);
            Assert.Throws<InvalidOperationException>(() => { pool.Return(handle); });
            pool.Dispose();
        }

        [Test]
        public void Dispose_Success()
        {
            var pool = new ObjectPool<DummyPooledObj>(() => CreateDummyPooledObjAsync(10));
            pool.WarmupAsync(3).Wait();
            pool.Dispose();
            Assert.That(pool.Capacity, Is.Zero);
            Assert.That(pool.WaitingObjectsCount, Is.Zero);
            Assert.That(pool.IsDisposed, Is.True);
        }

        [Test]
        public void Dispose_AlreadyDisposed_NothingHappens()
        {
            var pool = new ObjectPool<DummyPooledObj>(() => CreateDummyPooledObjAsync(10));
            pool.WarmupAsync(3).Wait();
            pool.Dispose();
            pool.Dispose();
        }

        [Test]
        public void Dispose_CallDisposeActions()
        {
            var disposeActionCount = 0;
            var pool = new ObjectPool<DummyPooledObj>(
                () => CreateDummyPooledObjAsync(10),
                x => disposeActionCount++);
            pool.WarmupAsync(3).Wait();
            pool.Use();
            pool.Dispose();
            Assert.That(disposeActionCount, Is.EqualTo(3));
        }

        private static async Task<DummyPooledObj> CreateDummyPooledObjAsync(int processingTimeMillis)
        {
            return await Task.Run(() =>
            {
                Thread.Sleep(processingTimeMillis);
                return new DummyPooledObj();
            }).ConfigureAwait(false);
        }
    }
}