using System;

namespace Addler.Runtime.Foundation.EventDispatcher
{
    public sealed class AnonymousEventDispatcher : IEventDispatcher
    {
        public event Action OnDispatch;

        public void Dispatch()
        {
            OnDispatch?.Invoke();
        }
    }
}