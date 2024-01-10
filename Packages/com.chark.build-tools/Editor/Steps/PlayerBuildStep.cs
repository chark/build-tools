using System.Collections.Generic;
using CHARK.BuildTools.Editor.Context;
using CHARK.BuildTools.Editor.Utilities;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace CHARK.BuildTools.Editor.Steps
{
    [CreateAssetMenu(
        fileName = CreateAssetMenuConstants.BaseFileName + nameof(PlayerBuildStep),
        menuName = CreateAssetMenuConstants.BaseMenuName + "/Steps/Player Build Step",
        order = CreateAssetMenuConstants.BaseOrder
    )]
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
        private BuildTarget buildTarget = BuildTarget.StandaloneWindows64;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Build", Expanded = true)]
#endif
        [SerializeField]
        private BuildOptions buildOptions = BuildOptions.None
            | BuildOptions.ShowBuiltPlayer
            | BuildOptions.AutoRunPlayer;

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

        protected override void Execute(IBuildContext context)
        {
            context.AddVariable("buildName", () => Application.productName);
            context.AddVariable("buildVersion", () => Application.version);
            context.AddVariable("buildDate", () => context.BuildDateTime.ToString(dateTimeFormat));
            context.AddVariable("buildTarget", () => buildTarget.ToString());

            var options = CreateBuildPlayerOptions(context);
            var report = BuildPipeline.BuildPlayer(options);

            LogBuildReport(report);

            if (report.summary.totalErrors > 0)
            {
                return;
            }

            context.AddArtifact(options.locationPathName);
        }

        private BuildPlayerOptions CreateBuildPlayerOptions(IBuildContext context)
        {
            return new BuildPlayerOptions
            {
                scenes = GetEnabledScenePaths(),
                locationPathName = GetBuildPath(context),
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

        private string GetBuildPath(IBuildContext context)
        {
            return context.ReplaceVariables(buildPath);
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
                Logging.LogError(message, this);
            }
            else
            {
                Logging.LogDebug(message, this);
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
