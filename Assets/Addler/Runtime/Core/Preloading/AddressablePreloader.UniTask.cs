#if !ADDLER_DISABLE_PRELOADING && ADDLER_UNITASK_SUPPORT
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Addler.Runtime.Core.Preloading
{
    public sealed partial class AddressablePreloader
    {
        public UniTask PreloadKeyAsync<TObject>(object key, IProgress<float> progress = null)
        {
            return PreloadKey<TObject>(key, progress).ToUniTask();
        }

        public UniTask PreloadKeysAsync<TObject>(IEnumerable<object> keys, IProgress<float> progress = null)
        {
            return PreloadKeys<TObject>(keys, progress).ToUniTask();
        }
    }
}
#endif
