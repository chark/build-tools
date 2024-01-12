using System;
using System.Collections.Generic;

namespace CHARK.BuildTools.Editor.Steps
{
    public interface IBuildStep
    {
        public readonly struct Artifact
        {
            public IBuildStep BuildStep { get; }

            public string Name { get; }

            public string Path { get; }

            public Artifact(IBuildStep buildStep, string name, string path)
            {
                BuildStep = buildStep;
                Name = name;
                Path = path;
            }
        }

        /// <summary>
        /// Enumerable of all artifact paths from all build steps.
        /// </summary>
        public IEnumerable<Artifact> Artifacts { get; }

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
        /// Enumerable of paths from build steps with given <paramref name="buildStepNames"/>.
        /// </returns>
        public IEnumerable<Artifact> GetArtifacts(IEnumerable<string> buildStepNames);

        /// <returns>
        /// Enumerable of paths from build step with given <paramref name="buildStepName"/>.
        /// </returns>
        public IEnumerable<Artifact> GetArtifacts(string buildStepName);

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
