#if !ADDLER_DISABLE_PRELOADING
using System.Collections.Generic;
using System.Linq;
using Addler.Runtime.Core.Preloading;
using Development.Demo.Shared.Scripts;
using Development.Shared.Runtime.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace Development.Demo.Runtime.Preloading.Scripts
{
    public sealed class PreloadingDemo : MonoBehaviour
    {
        public ButtonListView preloadListView;
        public ButtonListView getPreloadedListView;
        private readonly AddressablePreloader _preloader = new AddressablePreloader();
        private Transform _instanceRoot;

        private void Start()
        {
            AddPreloadButtons();
            AddGetAssetButtons();
            _instanceRoot = new GameObject("PreloadedAssets").transform;
        }

        private void OnDestroy()
        {
            _preloader.Dispose();
        }

        private void AddPreloadButtons()
        {
            foreach (var address in Addresses.EnumerateAll())
                AddPreloadKeyButton(address, address);

            var plasticAddresses = Addresses.EnumerateSpherePlastic().ToArray();
            AddPreloadKeysButton(plasticAddresses, "All Plastic");

            var metalAddresses = Addresses.EnumerateSphereMetal().ToArray();
            AddPreloadKeysButton(metalAddresses, "All Metal");

            foreach (var label in LabelNames.EnumerateAll())
                AddPreloadKeyButton(label, $"{label} (Label)");
        }

        private void AddGetAssetButtons()
        {
            foreach (var address in Addresses.EnumerateAll())
                AddGetAssetButton(address, address);

            foreach (var label in LabelNames.EnumerateAll())
                AddGetAssetsButton(label, $"{label} (Label)");
        }

        private void AddPreloadKeyButton(object key, string label)
        {
            var button = preloadListView.AddItem();
            button.GetComponentInChildren<Text>().text = label;
            button.onClick.AddListener(() =>
            {
                var routine = _preloader.PreloadKey<GameObject>(key);
                StartCoroutine(routine);
            });
        }

        private void AddPreloadKeysButton(IEnumerable<object> keys, string label)
        {
            var button = preloadListView.AddItem();
            button.GetComponentInChildren<Text>().text = label;
            button.onClick.AddListener(() =>
            {
                var routine = _preloader.PreloadKeys<GameObject>(keys);
                StartCoroutine(routine);
            });
        }

        private void AddGetAssetButton(object key, string label)
        {
            var button = getPreloadedListView.AddItem();
            button.GetComponentInChildren<Text>().text = label;
            button.onClick.AddListener(() =>
            {
                var obj = _preloader.GetAsset<GameObject>(key);
                Instantiate(obj);
            });
        }

        private void AddGetAssetsButton(object key, string label)
        {
            var button = getPreloadedListView.AddItem();
            button.GetComponentInChildren<Text>().text = label;
            button.onClick.AddListener(() =>
            {
                var objects = _preloader.GetAssets<GameObject>(key);
                foreach (var obj in objects)
                    Instantiate(obj);
            });
        }

        private void Instantiate(GameObject obj)
        {
            var instance = Instantiate(obj, _instanceRoot);
            var position = Random.insideUnitSphere * 4f;
            instance.transform.localPosition = position;
        }
    }
}
#endif
