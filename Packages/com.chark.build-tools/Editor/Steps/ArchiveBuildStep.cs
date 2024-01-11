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
        [Sirenix.OdinInspector.ValueDropdown(
            nameof(BuildStepNames)
        )]
#endif
        [SerializeField]
        private List<string> archiveBuildStepNames = new();

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Archive", Expanded = true)]
        [Sirenix.OdinInspector.Required]
        [Sirenix.OdinInspector.FolderPath]
#endif
        [SerializeField]
        private string archivePath = "Builds/{buildTarget}-{buildVersion}-{buildDate}.zip";

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

        public override void Execute()
        {
            // TODO: get build step for each build step name
            // TODO: replace variables using that build step (prolly need to relax replacement)
            var artifacts = GetArtifactPaths(Array.Empty<IBuildStep>()).ToList(); // TODO: names
            foreach (var path in artifacts)
            {
                var src = Path.GetDirectoryName(path);
                var dst = ReplaceVariables(archivePath);

                Archive(src, dst);

                AddArtifact(src, dst);
            }
        }

        private void Archive(string sourceDirectoryPath, string destinationFilePath)
        {
            var fastZip = new FastZip
            {
                CreateEmptyDirectories = true,
                CompressionLevel = archiveCompressionLevel,
            };

            var directoryFilter = new ArchiveSuffixFilter(ReplaceVariables(ignoreDirectorySuffixes));
            var fileFilter = new ArchiveSuffixFilter(ReplaceVariables(ignoreFileSuffixes));

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

        private sealed class ArchiveSuffixFilter : IScanFilter
        {
            private readonly ICollection<string> ignoreDirectorySuffixes;

            public ArchiveSuffixFilter(IEnumerable<string> ignoreDirectorySuffixes)
            {
                this.ignoreDirectorySuffixes = ignoreDirectorySuffixes.ToList();
            }

            public bool IsMatch(string name)
            {
                foreach (var ignoreDirectorySuffix in ignoreDirectorySuffixes)
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
