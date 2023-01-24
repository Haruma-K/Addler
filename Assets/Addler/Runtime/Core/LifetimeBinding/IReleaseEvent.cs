using System;

namespace Addler.Runtime.Core.LifetimeBinding
{
    public interface IReleaseEvent
    {
        event Action Dispatched;
    }
}
