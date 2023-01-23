#if !ADDLER_DISABLE_POOLING && ADDLER_UNITASK_SUPPORT
using System;
using Cysharp.Threading.Tasks;

namespace Addler.Runtime.Core.Pooling
{
    public sealed partial class AddressablePool
    {
        public UniTask WarmupAsync(int capacity, IProgress<float> progress = null)
        {
            return Warmup(capacity, progress).ToUniTask();
        }
    }
}
#endif
