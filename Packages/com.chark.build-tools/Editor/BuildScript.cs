using System;
using System.Linq;
using CHARK.BuildTools.Editor.Utilities;
using UnityEditor;

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

            // TODO: fails due to SOs being destroyed after build...
            // TODO: https://forum.unity.com/threads/how-to-keep-in-memory-scriptableobject-alive-after-buildpipeline-buildplayer.848575/
            configuration.Build();
        }
    }
}
