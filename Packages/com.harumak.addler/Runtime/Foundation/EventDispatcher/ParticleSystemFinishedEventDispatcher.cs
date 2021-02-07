using System;
using UnityEngine;

namespace Addler.Runtime.Foundation.EventDispatcher
{
    /// <summary>
    ///     <see cref="IEventDispatcher" /> that dispatches an event when the ParticleSystem finishes.
    /// </summary>
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleSystemFinishedEventDispatcher : MonoBehaviour, IEventDispatcher
    {
        private bool _isAliveAtLastFrame;
        private ParticleSystem _particleSystem;

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        private void LateUpdate()
        {
            var isAlive = _particleSystem.IsAlive(true);
            if (_isAliveAtLastFrame && !isAlive)
            {
                OnDispatch?.Invoke();
            }

            _isAliveAtLastFrame = isAlive;
        }

        public event Action OnDispatch;
    }
}