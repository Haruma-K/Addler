using System;
using UnityEngine;

namespace Addler.Runtime.Core.LifetimeBinding
{
    /// <summary>
    ///     <see cref="IReleaseEvent" /> that release when the GameObject is destroyed.
    /// </summary>
    public sealed class MonoBehaviourBasedReleaseEvent : MonoBehaviour, IReleaseEvent
    {
        private void OnDestroy()
        {
            ReleasedInternal?.Invoke();
        }

        event Action IReleaseEvent.Dispatched
        {
            add => ReleasedInternal += value;
            remove => ReleasedInternal -= value;
        }

        private event Action ReleasedInternal;
    }
}
