using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using Object = UnityEngine.Object;

namespace CHARK.BuildTools.Editor.Utilities
{
    /// <summary>
    /// General utilities for interacting with <see cref="BuildConfiguration"/> assets in
    /// Editor scripts.
    /// </summary>
    internal static class BuildToolsEditorUtilities
    {
        /// <summary>
        /// Called when editor state changes and Build Tools editors should reload. This event
        /// will be called when:
        /// <ul>
        ///   <li>An asset gets deleted or created</li>
        /// </ul>
        /// </summary>
        internal static event Action OnEditorStateChanged;

        /// <returns>
        /// Enumerable of all <see cref="BuildConfiguration"/> in the project.
        /// </returns>
        internal static IEnumerable<BuildConfiguration> GetBuildConfigurations()
        {
            return AssetDatabase
                .FindAssets($"t:{typeof(BuildConfiguration)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<BuildConfiguration>)
                .OrderBy(collection => collection.GetDisplayOrder())
                .ThenBy(collection => collection.Name)
                .ToList();
        }

        /// <returns>
        /// The sort order at which to display the given <paramref name="configuration"/> in Editor
        /// lists.
        /// </returns>
        internal static int GetDisplayOrder(this BuildConfiguration configuration)
        {
            var key = GeDisplayOrderKey(configuration);
            return EditorPrefs.GetInt(key, 0);
        }

        /// <summary>
        /// Set the sort order at which to display the given <paramref name="configuration"/> in
        /// Editor lists.
        /// </summary>
        internal static void SetDisplayOrder(
            this BuildConfiguration configuration,
            int order
        )
        {
            var key = configuration.GeDisplayOrderKey();
            EditorPrefs.SetInt(key, order);
        }

        /// <summary>
        /// Invoke <see cref="OnEditorStateChanged"/>.
        /// </summary>
        internal static void TriggerEditorStateChange()
        {
            OnEditorStateChanged?.Invoke();
        }

        private static string GeDisplayOrderKey(this Object collection)
        {
            var prefix = typeof(BuildConfiguration).FullName;
            const string function = nameof(GeDisplayOrderKey);
            var target = collection.GetGuid();

            return $"{prefix}_{function}_{target}";
        }

        private static string GetGuid(this Object obj)
        {
            var assetPath = AssetDatabase.GetAssetPath(obj);
            return AssetDatabase.AssetPathToGUID(assetPath);
        }
    }
}
