using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CHARK.BuildTools.Editor.Utilities;
using Unity.SharpZipLib.Core;
using Unity.SharpZipLib.Zip;
using Unity.SharpZipLib.Zip.Compression;
using UnityEditor;
using UnityEngine;

namespace CHARK.BuildTools.Editor.Steps
{
    [Serializable]
    internal sealed class ArchiveBuildStep : BuildStep
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Archive", Expanded = true)]
#else
        [Header("Archive")]
#endif
        [SerializeField]
        private Deflater.CompressionLevel archiveCompressionLevel = Deflater.CompressionLevel.BEST_COMPRESSION;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Archive", Expanded = true)]
        [Sirenix.OdinInspector.Required]
        [Sirenix.OdinInspector.FolderPath]
#endif
        [SerializeField]
        private string archivePath = "Builds/{buildTarget}-{buildVersion}-{buildDate}.zip";

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Archive", Expanded = true)]
        [Sirenix.OdinInspector.PropertySpace]
#endif
        [SerializeField]
        private bool isCreateSingleArchive;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Archive", Expanded = true)]
#endif
        [SerializeField]
        private bool isArchiveAllArtifacts;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Archive", Expanded = true)]
        [Sirenix.OdinInspector.Required]
        [Sirenix.OdinInspector.PropertySpace]
        [Sirenix.OdinInspector.HideIf(nameof(isArchiveAllArtifacts))]
        [Sirenix.OdinInspector.ValueDropdown(
            nameof(OtherBuildStepNames)
        )]
#endif
        [SerializeField]
        private List<string> archiveBuildSteps = new();

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Filtering", Expanded = true)]
#else
        [Header("Filtering")]
#endif
        [SerializeField]
        private List<string> ignoreDirectorySuffixes = new()
        {
            "_BurstDebugInformation_DoNotShip",
        };

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Filtering", Expanded = true)]
#endif
        [SerializeField]
        private List<string> ignoreFileSuffixes = new();

        protected override IEnumerable<string> ConsumesVariables => GetVariableNames(archivePath, isNormalize: false)
            .Append(GetVariableNames(ignoreDirectorySuffixes, isNormalize: false))
            .Append(GetVariableNames(ignoreFileSuffixes, isNormalize: false))
            .Distinct()
            .ToList();

        private IEnumerable<string> OtherBuildStepNames
        {
            get
            {
                return Context
                    .BuildSteps
                    .Where(buildStep => buildStep != this)
                    .Select(buildStep => buildStep.Name);
            }
        }

        protected override void OnExecuted()
        {
            var artifacts = isArchiveAllArtifacts ? Artifacts : GetArtifacts(archiveBuildSteps);
            foreach (var artifact in artifacts)
            {
                var src = Path.GetDirectoryName(artifact.Path);
                var dst = artifact.BuildStep.ReplaceVariables(archivePath);

                Archive(src, dst);

                AddArtifact(src, dst);
            }
        }

        private void Archive(
            string sourceDirectoryPath,
            string destinationFilePath
        )
        {
            var fastZip = new FastZip
            {
                CreateEmptyDirectories = true,
                CompressionLevel = archiveCompressionLevel,
            };

            var directoryFilter = new ArchiveFilter(
                ignoreNameSuffixes: ReplaceVariables(ignoreDirectorySuffixes)
            );

            var fileFilter = new ArchiveFilter(
                ignoreNameSuffixes: ReplaceVariables(ignoreFileSuffixes)
            );

            EditorUtility.DisplayProgressBar(Name, destinationFilePath, 1f);

            try
            {
                fastZip.CreateZip(
                    sourceDirectory: sourceDirectoryPath,
                    zipFileName: destinationFilePath,
                    recurse: true,
                    directoryFilter: directoryFilter,
                    fileFilter: fileFilter
                );
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private sealed class ArchiveFilter : IScanFilter
        {
            private readonly ICollection<string> ignoreNameSuffixes;

            public ArchiveFilter(IEnumerable<string> ignoreNameSuffixes)
            {
                this.ignoreNameSuffixes = ignoreNameSuffixes.ToList();
            }

            public bool IsMatch(string name)
            {
                foreach (var ignoreDirectorySuffix in ignoreNameSuffixes)
                {
                    if (name.EndsWith(ignoreDirectorySuffix))
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }
}
