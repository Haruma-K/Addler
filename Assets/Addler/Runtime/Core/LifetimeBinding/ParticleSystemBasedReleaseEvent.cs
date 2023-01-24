using System;
using UnityEngine;

namespace Addler.Runtime.Core.LifetimeBinding
{
    /// <summary>
    ///     <see cref="IReleaseEvent" /> that release when the ParticleSystem is finished.
    /// </summary>
    [RequireComponent(typeof(ParticleSystem))]
    public sealed class ParticleSystemBasedReleaseEvent : MonoBehaviour, IReleaseEvent
    {
        [SerializeField] private ParticleSystem particle;
        private bool _isAliveAtLastFrame;

        private void Awake()
        {
            if (particle == null)
                particle = GetComponent<ParticleSystem>();
        }

        private void Reset()
        {
            particle = GetComponent<ParticleSystem>();
        }

        private void LateUpdate()
        {
            var isAlive = particle.IsAlive(true);
            if (_isAliveAtLastFrame && !isAlive)
                ReleasedInternal?.Invoke();

            _isAliveAtLastFrame = isAlive;
        }

        event Action IReleaseEvent.Dispatched
        {
            add => ReleasedInternal += value;
            remove => ReleasedInternal -= value;
        }

        private event Action ReleasedInternal;
    }
}
