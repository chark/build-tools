using System;
using System.Collections.Generic;
using CHARK.BuildTools.Editor.Steps;

namespace CHARK.BuildTools.Editor.Contexts
{
    internal sealed class DefaultBuildContext : IBuildContext
    {
        public DateTime BuildDateTime => DateTime.MinValue;

        public IEnumerable<IBuildArtifact> Artifacts { get; } = Array.Empty<IBuildArtifact>();

        public IEnumerable<IBuildStep> BuildSteps { get; } = Array.Empty<IBuildStep>();

        internal static DefaultBuildContext Instance { get; } = new();

        private DefaultBuildContext()
        {
        }

        public IEnumerable<string> ReplaceVariables(IBuildStep buildStep, IEnumerable<string> templates)
        {
            return templates;
        }

        public string ReplaceVariables(IBuildStep buildStep, string template)
        {
            return template;
        }

        public bool TryGetBuildStep(string buildStepName, out IBuildStep buildStep)
        {
            buildStep = default;
            return false;
        }

        public bool TryGetVariable<T>(IBuildStep buildStep, string variableName, out T value)
        {
            value = default;
            return false;
        }

        public void AddVariable(IBuildStep buildStep, string variableName, object variableValue)
        {
        }

        public IEnumerable<IBuildArtifact> GetArtifacts(string buildStepName)
        {
            return Array.Empty<IBuildArtifact>();
        }

        public IEnumerable<IBuildArtifact> GetArtifacts(IBuildStep buildStep)
        {
            return Array.Empty<IBuildArtifact>();
        }

        public void AddArtifact(IBuildStep buildStep, string artifactPath)
        {
        }

        public void AddArtifact(IBuildStep buildStep, string artifactName, string artifactPath)
        {
        }

        public object Clone()
        {
            return DefaultBuildContext.Instance;
        }
    }
}
