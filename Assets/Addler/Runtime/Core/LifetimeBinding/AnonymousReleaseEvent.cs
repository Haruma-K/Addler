using System;

namespace Addler.Runtime.Core.LifetimeBinding
{
    public sealed class AnonymousReleaseEvent : IReleaseEvent
    {
        event Action IReleaseEvent.Dispatched
        {
            add => ReleasedInternal += value;
            remove => ReleasedInternal -= value;
        }

        private event Action ReleasedInternal;

        public void Release()
        {
            ReleasedInternal?.Invoke();
        }
    }
}
