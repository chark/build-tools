using System;
using System.Collections.Generic;
using CHARK.BuildTools.Editor.Utilities;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace CHARK.BuildTools.Editor.Steps
{
    [Serializable]
    internal sealed class PlayerBuildStep : BuildStep
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Build", Expanded = true)]
        [Sirenix.OdinInspector.ListDrawerSettings(
            DefaultExpandedState = true,
            ShowPaging = false
        )]
#else
        [Header("Build")]
#endif
        [SerializeField]
        private BuildTarget buildTarget = BuildTarget.NoTarget;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Build", Expanded = true)]
#endif
        [SerializeField]
        private BuildOptions buildOptions = BuildOptions.None;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Paths", Expanded = true)]
        [Sirenix.OdinInspector.Required]
        [Sirenix.OdinInspector.FolderPath]
#else
        [Header("Paths")]
#endif
        [SerializeField]
        private string buildPath = "Builds/{buildTarget}-{buildVersion}-{buildDate}/{buildName}.exe";

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Paths", Expanded = true)]
        [Sirenix.OdinInspector.Required]
#endif
        [SerializeField]
        private string dateTimeFormat = "yyyyMMHHmmss";

        protected override IEnumerable<string> ProducesVariables { get; } = new[]
        {
            "buildName",
            "buildVersion",
            "buildDate",
            "buildTarget",
        };

        protected override void OnExecuted()
        {
            AddVariable("buildName", Application.productName);
            AddVariable("buildVersion", Application.version);
            AddVariable("buildDate", BuildDateTime.ToString(dateTimeFormat));
            AddVariable("buildTarget", buildTarget.ToString());

            var options = CreateBuildPlayerOptions();
            var report = BuildPipeline.BuildPlayer(options);

            LogBuildReport(report);

            if (report.summary.totalErrors > 0)
            {
                return;
            }

            AddArtifact(options.locationPathName);
        }

        private BuildPlayerOptions CreateBuildPlayerOptions()
        {
            return new BuildPlayerOptions
            {
                scenes = GetEnabledScenePaths(),
                locationPathName = ReplaceVariables(buildPath),
                options = buildOptions,
                target = buildTarget,
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

        private void LogBuildReport(BuildReport report)
        {
            var summary = report.summary;
            var result = summary.result;

            var sizeGb = summary.totalSize / 1024f / 1024f / 1024f;

            var resultString = GetBuildResultString(summary.totalErrors, result);
            var message = ""
                + $"\nBuild for {summary.platform.ToString()} {resultString}!"
                + $"\nOutput Path: {summary.outputPath}"
                + $"\nDuration: {summary.totalTime.ToString()}"
                + $"\nWarnings: {summary.totalWarnings.ToString()}"
                + $"\nErrors: {summary.totalErrors.ToString()}"
                + $"\nSize: {sizeGb:F2} GB";

            if (summary.totalErrors > 0)
            {
                Logging.LogError(message, GetType());
            }
            else
            {
                Logging.LogDebug(message, GetType());
            }
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
    }
}
