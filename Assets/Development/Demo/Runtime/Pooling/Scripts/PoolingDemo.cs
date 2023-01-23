#if !ADDLER_DISABLE_POOLING
using System.Collections.Generic;
using System.Linq;
using Addler.Runtime.Core.Pooling;
using Cysharp.Threading.Tasks;
using Development.Demo.Shared.Scripts;
using Development.Shared.Runtime.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace Development.Demo.Runtime.Pooling.Scripts
{
    public sealed class PoolingDemo : MonoBehaviour
    {
        public ButtonListView menuListView;

        private readonly List<PooledObject> _pooledObjectHandles =
            new List<PooledObject>();

        private AddressablePool _pool;

        private void Start()
        {
            _pool = new AddressablePool(Addresses.SphereRedPlastic);

            var warmupButton = menuListView.AddItem();
            warmupButton.GetComponentInChildren<Text>().text = "Warmup";
            warmupButton.onClick.AddListener(() => _pool.WarmupAsync(5).Forget());

            var useButton = menuListView.AddItem();
            useButton.GetComponentInChildren<Text>().text = "Use";
            useButton.onClick.AddListener(() =>
            {
                var pooledObjectOperation = _pool.Use();
                _pooledObjectHandles.Add(pooledObjectOperation);
                var instance = pooledObjectOperation.Instance;
                instance.transform.SetParent(transform);
                instance.transform.position = Random.insideUnitSphere * 4;
            });

            var releaseButton = menuListView.AddItem();
            releaseButton.GetComponentInChildren<Text>().text = "Return";
            releaseButton.onClick.AddListener(() =>
            {
                var pooledObjectOperation = _pooledObjectHandles.First();
                _pooledObjectHandles.RemoveAt(0);
                pooledObjectOperation.Dispose();
            });

            var releaseAllButton = menuListView.AddItem();
            releaseAllButton.GetComponentInChildren<Text>().text = "Return All";
            releaseAllButton.onClick.AddListener(() =>
            {
                foreach (var pooledObjectOperation in _pooledObjectHandles)
                    pooledObjectOperation.Dispose();

                _pooledObjectHandles.Clear();
            });
        }

        private void OnDestroy()
        {
            _pool.Dispose();
        }
    }
}
#endif
