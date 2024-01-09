using System.Collections.Generic;
using System.IO;
using CHARK.BuildTools.Editor.Context;
using CHARK.BuildTools.Editor.Utilities;
using Unity.SharpZipLib.Core;
using Unity.SharpZipLib.Zip;
using Unity.SharpZipLib.Zip.Compression;
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

        protected override void Execute(IBuildContext context)
        {
            var src = Path.GetDirectoryName(context.GetArtifactPath("buildPath"));
            var dst = context.ReplaceVariables(archivePath);

            Archive(src, dst);
        }

        private void Archive(string sourceDirectoryPath, string destinationFilePath)
        {
            var fastZip = new FastZip
            {
                CreateEmptyDirectories = true,
                CompressionLevel = archiveCompressionLevel,
            };

            fastZip.CreateZip(
                sourceDirectory: sourceDirectoryPath,
                zipFileName: destinationFilePath,
                recurse: true,
                directoryFilter: new ArchiveSuffixFilter(ignoreDirectorySuffixes),
                fileFilter: new ArchiveSuffixFilter(ignoreFileSuffixes)
            );
        }

        private sealed class ArchiveSuffixFilter : IScanFilter
        {
            private readonly ICollection<string> ignoreDirectorySuffixes;

            public ArchiveSuffixFilter(ICollection<string> ignoreDirectorySuffixes)
            {
                this.ignoreDirectorySuffixes = ignoreDirectorySuffixes;
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
