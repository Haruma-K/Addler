using UnityEditor;
using UnityEngine;

namespace Development.Shared.Editor.Scripts
{
    public static class DevelopmentMenuItems
    {
        [MenuItem("Addler/Development/Clear Cache")]
        public static void ClearCache()
        {
            Caching.ClearCache();
        }
    }
}
