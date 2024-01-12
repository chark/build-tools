using System;
using System.Collections.Generic;
using CHARK.BuildTools.Editor.Steps;

namespace CHARK.BuildTools.Editor.Contexts
{
    internal interface IBuildContext : ICloneable
    {
        /// <summary>
        /// Date when the build has started.
        /// </summary>
        public DateTime BuildDateTime { get; }

        /// <summary>
        /// Enumerable containing all artifacts produced during this build (so far).
        /// </summary>
        public IEnumerable<IBuildArtifact> Artifacts { get; }

        /// <summary>
        /// Enumerable of build steps present during this build.
        /// </summary>
        public IEnumerable<IBuildStep> BuildSteps { get; }

        /// <returns>
        /// Enumerable with interpolated variables. The variables are replaced using the data from
        /// provided <paramref name="buildStep"/>. If no variable data is found, other build steps
        /// are looked up as a fallback.
        /// </returns>
        public IEnumerable<string> ReplaceVariables(IBuildStep buildStep, IEnumerable<string> templates);

        /// <returns>
        /// String with interpolated variables. The variables are replaced using the data from
        /// provided <paramref name="buildStep"/>. If no variable data is found, other build steps
        /// are looked up as a fallback.
        /// </returns>
        public string ReplaceVariables(IBuildStep buildStep, string template);

        /// <returns>
        /// <c>true</c> if <paramref name="buildStep"/> is retrieved using
        /// <paramref name="buildStepName"/> or <c>false</c> otherwise.
        /// </returns>
        public bool TryGetBuildStep(string buildStepName, out IBuildStep buildStep);

        /// <returns>
        /// <c>true</c> if <paramref name="buildStep"/> is retrieved using
        /// <paramref name="variableName"/> or <c>false</c> otherwise.
        /// </returns>
        public bool TryGetVariable<T>(IBuildStep buildStep, string variableName, out T value);

        /// <summary>
        /// Add a new variable with <paramref name="variableValue"/> and
        /// <paramref name="variableName"/> to given <paramref name="buildStep"/>.
        /// </summary>
        public void AddVariable(IBuildStep buildStep, string variableName, object variableValue);

        /// <returns>
        /// Enumerable of artifacts retrieved from build step with given <paramref name="buildStepName"/>.
        /// </returns>
        public IEnumerable<IBuildArtifact> GetArtifacts(string buildStepName);

        /// <returns>
        /// Enumerable of artifacts retrieved from given <paramref name="buildStep"/>.
        /// </returns>
        public IEnumerable<IBuildArtifact> GetArtifacts(IBuildStep buildStep);

        /// <summary>
        /// Add <paramref name="artifactPath"/> to given <paramref name="buildStep"/>.
        /// </summary>
        public void AddArtifact(IBuildStep buildStep, string artifactPath);

        /// <summary>
        /// Add <paramref name="artifactPath"/> with <paramref name="artifactName"/> to given
        /// <paramref name="buildStep"/>.
        /// </summary>
        public void AddArtifact(IBuildStep buildStep, string artifactName, string artifactPath);
    }
}
