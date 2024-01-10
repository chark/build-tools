using System.Collections.Generic;
using System.IO;
using System.Linq;
using CHARK.BuildTools.Editor.Context;
using CHARK.BuildTools.Editor.Utilities;
using Unity.SharpZipLib.Core;
using Unity.SharpZipLib.Zip;
using Unity.SharpZipLib.Zip.Compression;
using UnityEditor;
using UnityEngine;

namespace CHARK.BuildTools.Editor.Steps
{
    [CreateAssetMenu(
        fileName = CreateAssetMenuConstants.BaseFileName + nameof(ArchiveBuildStep),
        menuName = CreateAssetMenuConstants.BaseMenuName + "/Steps/Archive Build Step",
        order = CreateAssetMenuConstants.BaseOrder
    )]
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

        protected override IEnumerable<string> ConsumesVariables => GetVariableNames(archivePath)
            .Append(GetVariableNames(ignoreDirectorySuffixes))
            .Append(GetVariableNames(ignoreFileSuffixes))
            .Distinct()
            .ToList();

        protected override void Execute(IBuildContext context)
        {
            var artifacts = context.ArtifactPaths.ToList();
            foreach (var path in artifacts)
            {
                var src = Path.GetDirectoryName(path);
                var dst = context.ReplaceVariables(archivePath);

                Archive(src, dst, context);

                context.AddArtifact(src, dst);
            }
        }

        private void Archive(string sourceDirectoryPath, string destinationFilePath, IBuildContext context)
        {
            var fastZip = new FastZip
            {
                CreateEmptyDirectories = true,
                CompressionLevel = archiveCompressionLevel,
            };

            var directoryFilter = new ArchiveSuffixFilter(context.ReplaceVariables(ignoreDirectorySuffixes));
            var fileFilter = new ArchiveSuffixFilter(context.ReplaceVariables(ignoreFileSuffixes));

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
