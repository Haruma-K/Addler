using UnityEngine;

namespace Development.Demo.Shared.Scripts
{
    public abstract class ListView<TComponent> : MonoBehaviour
        where TComponent : Component

    {
        public TComponent prefab;
        public Transform content;

        public TComponent AddItem()
        {
            return Instantiate(prefab, content);
        }

        public void ClearItems()
        {
            foreach (Transform child in content)
                Destroy(child.gameObject);
        }

        public void RemoveItem(TComponent item)
        {
            Destroy(item.gameObject);
        }
    }
}
