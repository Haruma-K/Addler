using System;

namespace Addler.Runtime.Foundation.EventDispatcher
{
    /// <summary>
    ///     Interface that dispatches some kind of event.
    /// </summary>
    public interface IEventDispatcher
    {
        event Action OnDispatch;
    }
}