using System;
using System.Collections.Generic;

namespace CHARK.BuildTools.Editor.Steps
{
    public interface IBuildStep
    {
        /// <summary>
        /// Date when the build has started.
        /// </summary>
        public DateTime BuildDateTime { get; }

        /// <summary>
        /// Unique name of this build step.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Enumerable of artifacts produced by this build step.
        /// </summary>
        public IEnumerable<IBuildArtifact> Artifacts { get; }

        /// <returns>
        /// String with replaced variables specified in <paramref name="templates"/>.
        /// </returns>
        public IEnumerable<string> ReplaceVariables(IEnumerable<string> templates);

        /// <returns>
        /// String with replaced variables specified in <paramref name="template"/>.
        /// </returns>
        public string ReplaceVariables(string template);
    }
}
