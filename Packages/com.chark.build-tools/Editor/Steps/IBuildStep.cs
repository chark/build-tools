using System;
using System.Collections.Generic;

namespace CHARK.BuildTools.Editor.Steps
{
    public interface IBuildStep
    {
        /// <summary>
        /// Date time when this build was started.
        /// </summary>
        public DateTime BuildDateTime { get; }

        /// <summary>
        /// Unique name of this build step.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Unique names of available build steps (excluding this one).
        /// </summary>
        public IEnumerable<string> BuildStepNames { get; }

        /// <returns>
        /// Enumerable of variable names extracted from given <paramref name="templates"/>.
        /// </returns>
        public IEnumerable<string> GetVariableNames(IEnumerable<string> templates, bool isNormalize = true);

        /// <returns>
        /// Enumerable of variable names extracted from given <paramref name="template"/>.
        /// </returns>
        public IEnumerable<string> GetVariableNames(string template, bool isNormalize = true);

        /// <returns>
        /// String with replaced variables specified in <paramref name="templates"/>.
        /// </returns>
        public IEnumerable<string> ReplaceVariables(IEnumerable<string> templates);

        /// <returns>
        /// String with replaced variables specified in <paramref name="template"/>.
        /// </returns>
        public string ReplaceVariables(string template);

        /// <returns>
        /// Enumerable of paths from given <paramref name="buildSteps"/>.
        /// </returns>
        public IEnumerable<string> GetArtifactPaths(IEnumerable<IBuildStep> buildSteps);

        /// <returns>
        /// Enumerable of paths from given <paramref name="buildStep"/>.
        /// </returns>
        public IEnumerable<string> GetArtifactPaths(IBuildStep buildStep);

        /// <summary>
        /// Add build variable.
        /// </summary>
        public void AddVariable(string variableName, object variableValue);

        /// <summary>
        /// Add built <paramref name="artifactPath"/> to context with a generated name.
        /// </summary>
        public void AddArtifact(string artifactPath);

        /// <summary>
        /// Add built <paramref name="artifactPath"/> to context with given
        /// <paramref name="artifactName"/>.
        /// </summary>
        public void AddArtifact(string artifactName, string artifactPath);

        /// <summary>
        /// Execute this build step.
        /// </summary>
        public void Execute();
    }
}
