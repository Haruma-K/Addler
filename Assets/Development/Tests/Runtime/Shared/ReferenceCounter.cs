using System;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;

namespace Development.Tests.Runtime.Shared
{
    internal sealed class ReferenceCounter : IDisposable
    {
        public ReferenceCounter(string assetPath)
        {
            AssetPath = assetPath;
            Addressables.ResourceManager.RegisterDiagnosticCallback(DiagnosticCallback);
        }

        public string AssetPath { get; }
        public int ReferenceCount { get; private set; }

        public void Dispose()
        {
            Addressables.ResourceManager.UnregisterDiagnosticCallback(DiagnosticCallback);
        }

        private void DiagnosticCallback(ResourceManager.DiagnosticEventContext context)
        {
            if (context.Type != ResourceManager.DiagnosticEventType.AsyncOperationReferenceCount)
                return;

            if (context.Location == null)
                return;

            if (!context.Location.ToString().Equals(AssetPath))
                return;

            ReferenceCount = context.EventValue;
        }
    }
}
