using System;
using UnityEngine;

namespace Addler.Runtime.Foundation.EventDispatcher
{
    /// <summary>
    ///     <see cref="IEventDispatcher" /> that dispatches an event when the GameObject is destroyed.
    /// </summary>
    public class MonoBehaviourDestroyedEventDispatcher : MonoBehaviour, IEventDispatcher
    {
        public void OnDestroy()
        {
            OnDispatch?.Invoke();
        }

        public event Action OnDispatch;
    }
}