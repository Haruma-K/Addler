using System;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;

namespace Addler.Tests.Core
{
    public class ReferenceCounter : IDisposable
    {
        public ReferenceCounter(string location)
        {
            Location = location;
            Addressables.ResourceManager.RegisterDiagnosticCallback(DiagnosticCallback);
        }

        public string Location { get; }
        public int ReferenceCount { get; private set; }

        public void Dispose()
        {
            Addressables.ResourceManager.UnregisterDiagnosticCallback(DiagnosticCallback);
        }

        private void DiagnosticCallback(ResourceManager.DiagnosticEventContext context)
        {
            if (context.Type == ResourceManager.DiagnosticEventType.AsyncOperationReferenceCount)
            {
                if (context.Location == null)
                {
                    return;
                }

                if (!context.Location.ToString().Equals(Location))
                {
                    return;
                }

                ReferenceCount = context.EventValue;
            }
        }
    }
}