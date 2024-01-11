using System;
using System.Linq;
using CHARK.BuildTools.Editor.Utilities;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CHARK.BuildTools.Editor
{
    // ReSharper disable once UnusedType.Global
    internal static class BuildScript
    {
        // ReSharper disable once UnusedMember.Local
        private static void Build()
        {
            var args = CommandLineUtilities.GetCommandLineArguments();
            if (args.TryGetValue("configurationName", out var configurationName) == false)
            {
                Logging.LogError(
                    "Missing -configurationName argument",
                    typeof(BuildScript)
                );

                Environment.Exit(1);
                return;
            }

            var configuration = AssetDatabase
                .FindAssets($"t:{nameof(BuildConfiguration)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<BuildConfiguration>)
                .FirstOrDefault(config => config.name == configurationName);

            if (configuration == null)
            {
                Logging.LogError(
                    $"Could not find configuration with name: {configurationName}",
                    typeof(BuildScript)
                );

                Environment.Exit(1);
                return;
            }

            if (Application.isBatchMode)
            {
                // Build script fails in batch mode due to Scriptable Objects being destroyed
                // after BuildPipeline.BuildPlayer is called.
                //
                // For more info see:
                // https://forum.unity.com/threads/how-to-keep-in-memory-scriptableobject-alive-after-buildpipeline-buildplayer.848575/
                var temporaryConfiguration = Object.Instantiate(configuration);
                temporaryConfiguration.hideFlags |= HideFlags.DontUnloadUnusedAsset | HideFlags.DontSave;
                temporaryConfiguration.Build();
            }
            else
            {
                configuration.Build();
            }
        }
    }
}
