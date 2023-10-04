using UnityEditor;

namespace CHARK.BuildTools.Editor.Utilities
{
    /// <summary>
    /// Reloads assigned <see cref="BuildConfiguration"/> to
    /// <see cref="BuildToolsManagerWindow"/>.
    /// </summary>
    internal sealed class BuildToolsAssetPostprocessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths
        )
        {
            if (importedAssets.Length == 0 && deletedAssets.Length == 0)
            {
                return;
            }

            BuildToolsEditorUtilities.TriggerEditorStateChange();
        }
    }
}
