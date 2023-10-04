using System;
using System.Collections.Generic;
using CHARK.BuildTools.Editor.Utilities;
using UnityEngine;

namespace CHARK.BuildTools.Editor
{
    /// <summary>
    /// Configuration which represents a build.
    /// </summary>
    [CreateAssetMenu(
        fileName = CreateAssetMenuConstants.BaseFileName + nameof(BuildConfiguration),
        menuName = CreateAssetMenuConstants.BaseMenuName + "/Build Configuration",
        order = CreateAssetMenuConstants.BaseOrder
    )]
    internal sealed class BuildConfiguration : ScriptableObject, ICloneable
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("General", Expanded = true)]
#else
        [Header("General")]
#endif
        [SerializeField]
        private string configurationName;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Player", Expanded = true)]
#else
        [Header("Player")]
#endif
        [SerializeField]
        private bool isDevelopmentBuild;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Player", Expanded = true)]
#endif
        [SerializeField]
        private bool isShowBuiltPlayer = true;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Player", Expanded = true)]
#endif
        [SerializeField]
        private bool isAutoRunPlayer;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Player", Expanded = true)]
        [Sirenix.OdinInspector.PropertySpace]
        [Sirenix.OdinInspector.ListDrawerSettings(DefaultExpandedState = true)]
#endif
        [SerializeField]
        private List<BuildToolsTarget> buildTargets = new()
        {
            BuildToolsTarget.StandaloneWindows64,
        };

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Directories", Expanded = true)]
#else
        [Header("Directories")]
#endif
        [SerializeField]
        private string buildDirectory = "Builds";

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Directories", Expanded = true)]
#endif
        [SerializeField]
        private string archiveDirectory = "Builds";

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Naming", Expanded = true)]
#else
        [Header("Naming")]
#endif
        [SerializeField]
        private string buildNameOverride = "";

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Naming", Expanded = true)]
#endif
        [SerializeField]
        private string buildVersionOverride = "";

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Naming", Expanded = true)]
#endif
        [SerializeField]
        private string buildNameDelimiter = " ";

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Naming", Expanded = true)]
#endif
        [SerializeField]
        private string buildDateTimeFormat = "yyyyMMHHmm";

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Naming", Expanded = true)]
#endif
        [Space]
        [SerializeField]
        private bool isAppendVersionToBuildName = true;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Naming", Expanded = true)]
#endif
        [SerializeField]
        private bool isAppendPlatformToBuildName = true;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Naming", Expanded = true)]
#endif
        [SerializeField]
        private bool isAppendDateToBuildName = true;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Archive", Expanded = true)]
#else
        [Header("Archive")]
#endif
        [SerializeField]
        private bool isArchive;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Archive", Expanded = true)]
#endif
        [SerializeField]
        private bool isAppendVersionToArchiveName = true;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Archive", Expanded = true)]
#endif
        [SerializeField]
        private bool isAppendPlatformToArchiveName = true;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Archive", Expanded = true)]
#endif
        [SerializeField]
        private bool isAppendDateToArchiveName = true;

        /// <summary>
        /// User-friendly configuration name.
        /// </summary>
        internal string Name
        {
            get
            {
                if (string.IsNullOrWhiteSpace(configurationName))
                {
                    return GetDefaultName();
                }

                return configurationName;
            }
        }

        /// <summary>
        /// Is this a development build?
        /// </summary>
        internal bool IsDevelopmentBuild => isDevelopmentBuild;

        /// <summary>
        /// Should the player directory) (build directory) be shown when the build finishes?
        /// </summary>
        internal bool IsShowBuiltPlayer => isShowBuiltPlayer;

        /// <summary>
        /// Should the player automatically start when the build finishes?
        /// </summary>
        internal bool IsAutoRunPlayer
        {
            get => isAutoRunPlayer;
            set => isAutoRunPlayer = value;
        }

        /// <summary>
        /// Platform we're building for.
        /// </summary>
        internal IEnumerable<BuildToolsTarget> BuildTargets => buildTargets;

        /// <summary>
        /// Output directory.
        /// </summary>
        internal string BuildDirectory => buildDirectory;

        /// <summary>
        /// Archive output directory.
        /// </summary>
        internal string ArchiveDirectory => archiveDirectory;

        /// <summary>
        /// Build name, defaults to <see cref="Application.productName"/>.
        /// </summary>
        internal string BuildName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(buildNameOverride))
                {
                    return Application.productName;
                }

                return buildNameOverride;
            }
        }

        /// <summary>
        /// Build version, defaults to <see cref="Application.version"/>.
        /// </summary>
        internal string BuildVersion
        {
            get
            {
                if (string.IsNullOrWhiteSpace(buildVersionOverride))
                {
                    return Application.version;
                }

                return buildVersionOverride;
            }
        }

        /// <summary>
        /// Separator used when concentrating build name with date and other components.
        /// </summary>
        internal string BuildNameDelimiter => buildNameDelimiter;

        /// <summary>
        /// Date suffix format.
        /// </summary>
        internal string BuildDateTimeFormat => buildDateTimeFormat;

        /// <summary>
        /// Should <see cref="BuildVersion"/> be appended to the build name?
        /// </summary>
        internal bool IsAppendVersionToBuildName => isAppendVersionToBuildName;

        /// <summary>
        /// Should <see cref="BuildTargets"/> be appended to the build name?
        /// </summary>
        internal bool IsAppendPlatformToBuildName => isAppendPlatformToBuildName;

        /// <summary>
        /// Should build date (formatted with <see cref="BuildDateTimeFormat"/>) be appended to the
        /// build name?
        /// </summary>
        internal bool IsAppendDateToBuildName => isAppendDateToBuildName;

        /// <summary>
        /// Should the build be archived?
        /// </summary>
        internal bool IsArchive
        {
            get => isArchive;
            set => isArchive = value;
        }

        /// <summary>
        /// Should <see cref="BuildVersion"/> be appended to the archive name?
        /// </summary>
        internal bool IsAppendVersionToArchiveName => isAppendVersionToArchiveName;

        /// <summary>
        /// Should <see cref="BuildTargets"/> be appended to the archive name?
        /// </summary>
        internal bool IsAppendPlatformToArchiveName => isAppendPlatformToArchiveName;

        /// <summary>
        /// Should build date (formatted with <see cref="BuildDateTimeFormat"/>) be appended to the
        /// archive name?
        /// </summary>
        internal bool IsAppendDateToArchiveName => isAppendDateToArchiveName;

        public object Clone()
        {
            return MemberwiseClone();
        }

        private string GetDefaultName()
        {
            var title = string.Join(", ", buildTargets);
            var parts = new List<string>();

            if (isAutoRunPlayer)
            {
                parts.Add("autorun");
            }

            if (isArchive)
            {
                parts.Add("archive");
            }

            if (isDevelopmentBuild)
            {
                parts.Add("dev");
            }

            if (parts.Count > 0)
            {
                title += $" ({string.Join(", ", parts)})";
            }

            return title;
        }
    }
}
