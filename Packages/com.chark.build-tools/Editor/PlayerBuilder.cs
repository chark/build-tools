using System;
using System.Collections.Generic;
using System.IO;
using Unity.SharpZipLib.Core;
using Unity.SharpZipLib.Zip;
using Unity.SharpZipLib.Zip.Compression;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace CHARK.BuildTools.Editor
{
    // This script is based on: https://github.com/game-ci/documentation/blob/main/example/BuildScript.cs
    internal static class PlayerBuilder
    {
        private const Deflater.CompressionLevel ArchiveCompressionLevel =
            Deflater.CompressionLevel.BEST_COMPRESSION;

        private const string BurstDebugInformationDirectorySuffix = "_BurstDebugInformation_DoNotShip";
        private const string BuildArchiveExtension = "zip";

        /// <summary>
        /// Build application using given <paramref name="configuration"/> and log the results.
        /// </summary>
        internal static void BuildAndLog(BuildConfiguration configuration)
        {
            var dateTime = DateTime.Now;

            foreach (var buildTarget in configuration.BuildTargets)
            {
                var buildOptions = new BuildOptions(configuration)
                {
                    BuildTarget = buildTarget,
                    DateTime = dateTime,
                };

                try
                {
                    var buildReport = Build(buildOptions);
                    buildReport.Log();
                }
                catch (Exception exception)
                {
                    Debug.LogError($"Could not build configuration: {configuration.Name}");
                    Debug.LogException(exception);
                }
            }
        }

        private static BuildReport Build(BuildOptions buildOptions)
        {
            var playerOptions = CreateBuildPlayerOptions(buildOptions);
            var buildReport = BuildPipeline.BuildPlayer(playerOptions);

            if (buildOptions.Configuration.IsArchive)
            {
                Archive(buildOptions);
            }

            return buildReport;
        }

        private static void Log(this BuildReport report)
        {
            var summary = report.summary;
            var result = summary.result;

            var resultString = GetBuildResultString(summary.totalErrors, result);
            Debug.Log(
                ""
                + $"\nBuild for {summary.platform.ToString()} {resultString}!"
                + $"\nOutput Path: {summary.outputPath}"
                + $"\nDuration: {summary.totalTime.ToString()}"
                + $"\nWarnings: {summary.totalWarnings.ToString()}"
                + $"\nErrors: {summary.totalErrors.ToString()}"
                + $"\nSize: {((int)(summary.totalSize / 1024f / 1024f)).ToString()} MB"
            );
        }

        private static BuildPlayerOptions CreateBuildPlayerOptions(BuildOptions buildOptions)
        {
            return new BuildPlayerOptions
            {
                scenes = GetEnabledScenePaths(),
                locationPathName = GetBuildPath(buildOptions),
                options = CreateBuildOptions(buildOptions),
                target = GetUnityBuildTarget(buildOptions),
            };
        }

        private static string[] GetEnabledScenePaths()
        {
            var scenePaths = new List<string>();
            var scenes = EditorBuildSettings.scenes;

            foreach (var scene in scenes)
            {
                if (scene.enabled == false)
                {
                    continue;
                }

                var scenePath = scene.path;
                scenePaths.Add(scenePath);
            }

            return scenePaths.ToArray();
        }

        private static UnityEditor.BuildOptions CreateBuildOptions(BuildOptions buildOptions)
        {
            var settings = buildOptions.Configuration;
            var options = UnityEditor.BuildOptions.None;

            if (settings.IsDevelopmentBuild)
            {
                options |= UnityEditor.BuildOptions.Development;
            }

            if (settings.IsShowBuiltPlayer)
            {
                options |= UnityEditor.BuildOptions.ShowBuiltPlayer;
            }

            if (settings.IsAutoRunPlayer)
            {
                options |= UnityEditor.BuildOptions.AutoRunPlayer;
            }

            return options;
        }

        private static void Archive(BuildOptions buildOptions)
        {
            var buildPath = GetBuildPath(buildOptions);
            var buildDirectoryPath = Path.GetDirectoryName(buildPath);
            var archiveFilePath = GetArchiveFilePath(buildOptions);

            Archive(buildDirectoryPath, archiveFilePath);
        }

        private static string GetBuildPath(BuildOptions buildOptions)
        {
            var settings = buildOptions.Configuration;
            var parts = new List<string> { settings.BuildName };

            if (settings.IsAppendVersionToBuildName)
            {
                parts.Add(settings.BuildVersion);
            }

            var platform = GetBuildPlatformName(buildOptions.BuildTarget);
            if (settings.IsAppendPlatformToBuildName)
            {
                parts.Add(platform);
            }

            if (settings.IsAppendDateToBuildName)
            {
                parts.Add(buildOptions.DateTime.ToString(settings.BuildDateTimeFormat));
            }

            var buildFileExtension = GetFileExtension(buildOptions.BuildTarget);
            var buildFileName = $"{string.Join(settings.BuildNameDelimiter, parts)}";
            if (string.IsNullOrWhiteSpace(buildFileExtension) == false)
            {
                buildFileName = $"{buildFileName}.{buildFileExtension}";
            }

            var buildDirectory = settings.BuildDirectory;

            if (string.IsNullOrWhiteSpace(buildDirectory) == false)
            {
                return Path.Combine(buildDirectory, platform, buildFileName);
            }

            return Path.Combine(platform, buildFileName);
        }

        private static string GetArchiveFilePath(BuildOptions buildOptions)
        {
            var settings = buildOptions.Configuration;
            var parts = new List<string> { settings.BuildName };

            if (settings.IsAppendVersionToArchiveName)
            {
                parts.Add(settings.BuildVersion);
            }

            if (settings.IsAppendPlatformToArchiveName)
            {
                var targetName = GetBuildPlatformName(buildOptions.BuildTarget);
                parts.Add(targetName);
            }

            if (settings.IsAppendDateToArchiveName)
            {
                parts.Add(buildOptions.DateTime.ToString(settings.BuildDateTimeFormat));
            }

            var archiveFileName = $"{string.Join(settings.BuildNameDelimiter, parts)}." +
                                  $"{BuildArchiveExtension}";

            var archiveDirectory = settings.ArchiveDirectory;

            if (string.IsNullOrWhiteSpace(archiveDirectory) == false)
            {
                return Path.Combine(archiveDirectory, archiveFileName);
            }

            return archiveFileName;
        }

        private static string GetBuildPlatformName(BuildToolsTarget target)
        {
            return target.ToString();
        }

        private static BuildTarget GetUnityBuildTarget(BuildOptions buildOptions)
        {
            return Enum.Parse<BuildTarget>(buildOptions.BuildTarget.ToString());
        }

        private static string GetFileExtension(BuildToolsTarget target)
        {
            return target switch
            {
                BuildToolsTarget.StandaloneWindows => "exe",
                BuildToolsTarget.StandaloneWindows64 => "exe",
                BuildToolsTarget.Android => "apk",
                BuildToolsTarget.StandaloneLinux64 => "x86_64",
                BuildToolsTarget.StandaloneOSX => "app",
                BuildToolsTarget.iOS => "ipa",
                BuildToolsTarget.WebGL => "",
                _ => throw new Exception($"Unsupported target: {target}"),
            };
        }

        private static void Archive(string sourceDirectoryPath, string destinationFilePath)
        {
            var fastZip = new FastZip
            {
                CreateEmptyDirectories = true,
                CompressionLevel = ArchiveCompressionLevel,
            };

            fastZip.CreateZip(
                destinationFilePath,
                sourceDirectoryPath,
                true,
                new ArchiveFileFilter(),
                new ArchiveDirectoryFilter()
            );
        }

        private static string GetBuildResultString(int errorCount, BuildResult result)
        {
            if (errorCount > 0)
            {
                return "<color=red><b>Failed</b></color>";
            }

            switch (result)
            {
                case BuildResult.Succeeded:
                    return "<color=cyan><b>Succeeded</b></color>";
                case BuildResult.Failed:
                    return "<color=red><b>Failed</b></color>";
                case BuildResult.Cancelled:
                    return "<color=grey><b>Canceled</b></color>";
                default:
                    return $"<color=grey><b>{result.ToString()}</b></color>";
            }
        }

        private sealed class ArchiveFileFilter : IScanFilter
        {
            public bool IsMatch(string name)
            {
                return true;
            }
        }

        private sealed class ArchiveDirectoryFilter : IScanFilter
        {
            public bool IsMatch(string name)
            {
                return name.EndsWith(BurstDebugInformationDirectorySuffix) == false;
            }
        }
    }
}
