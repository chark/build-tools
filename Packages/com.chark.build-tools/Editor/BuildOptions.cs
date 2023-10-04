using System;

namespace CHARK.BuildTools.Editor
{
    internal sealed class BuildOptions
    {
        /// <summary>
        /// Configuration to build.
        /// </summary>
        internal BuildConfiguration Configuration { get; }

        /// <summary>
        /// Date time of the build. Useful to set this when doing bulk builds.
        /// </summary>
        internal DateTime DateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// Build target we're building right now.
        /// </summary>
        internal BuildToolsTarget BuildTarget { get; set; }

        internal BuildOptions(BuildConfiguration configuration)
        {
            Configuration = configuration;
        }
    }
}
