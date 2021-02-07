using System;
using System.Linq;
using UnityEngine;
using UnityEngine.TestTools;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build.DataBuilders;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine.TestTools.Utils;
#endif

namespace Addler.Tests.Core
{
    public class PrePostBuildProcess : IPrebuildSetup, IPostBuildCleanup
    {
        public void Cleanup()
        {
#if UNITY_EDITOR
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var playModeDataBuilderIndex = EditorPrefs.GetInt(ActivePlayModeDataBuilderIndexKey);
            settings.ActivePlayModeDataBuilderIndex = playModeDataBuilderIndex;
            var guid = AssetDatabase.AssetPathToGUID(TestStrings.TestPrefabPath);
            settings.RemoveAssetEntry(guid);
            var group = settings.FindGroup(TestGroupName);
            settings.RemoveGroup(group);
            AssetDatabase.DeleteAsset(TestStrings.TestFolderPath);
            AssetDatabase.SaveAssets();
#endif
        }

        public void Setup()
        {
#if UNITY_EDITOR

            var warningMessage =
                "This test requires a DiagnosticCallback. Make sure that Send Profiler Event is checked in Addressable Asset Settings.";
            Debug.LogWarning(warningMessage);

            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                throw new Exception($"{nameof(AddressableAssetSettingsDefaultObject)} is not found.");
            }

            EditorPrefs.SetInt(ActivePlayModeDataBuilderIndexKey, settings.ActivePlayModeDataBuilderIndex);
            var modeIndex = settings.DataBuilders.FindIndex(x => x is BuildScriptVirtualMode);
            settings.ActivePlayModeDataBuilderIndex = modeIndex;

            var groupTemplate = settings.GroupTemplateObjects
                .OfType<AddressableAssetGroupTemplate>()
                .First(x => x.HasSchema(typeof(BundledAssetGroupSchema)));

            var group = settings.CreateGroup(TestGroupName, false, false, false, null, groupTemplate.GetTypes());
            groupTemplate.ApplyToAddressableAssetGroup(group);
            var gameObj = Utils.CreatePrimitive(PrimitiveType.Sphere);
            if (!Directory.Exists(TestStrings.TestFolderPath))
            {
                Directory.CreateDirectory(TestStrings.TestFolderPath);
            }

            PrefabUtility.SaveAsPrefabAsset(gameObj, TestStrings.TestPrefabPath);
            var guid = AssetDatabase.AssetPathToGUID(TestStrings.TestPrefabPath);
            var entry = settings.CreateOrMoveEntry(guid, group);
            entry.address = TestStrings.TestPrefabAddress;

            AssetDatabase.SaveAssets();
#endif
        }
#if UNITY_EDITOR
        private string ActivePlayModeDataBuilderIndexKey => $"{GetType().Name}_ActivePlayModeDataBuilderIndex";
        private const string TestGroupName = "Test Group";
#endif
    }
}