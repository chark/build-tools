using System;
using System.Collections.Generic;
using System.Linq;
using CHARK.BuildTools.Editor.Steps;
using CHARK.BuildTools.Editor.Utilities;

namespace CHARK.BuildTools.Editor
{
    internal sealed class BuildContext : ICloneable
    {
        private readonly IDictionary<string, object> variables;
        private readonly IDictionary<IBuildStep, IDictionary<string, string>> artifacts;
        private readonly ICollection<IBuildStep> buildSteps;

        public DateTime BuildDateTime { get; }

        public IEnumerable<IBuildStep> BuildSteps => buildSteps;

        public BuildContext(DateTime buildDateTime, IEnumerable<IBuildStep> buildSteps) : this(
            variables: new Dictionary<string, object>(),
            artifacts: new Dictionary<IBuildStep, IDictionary<string, string>>(),
            buildSteps: buildSteps.ToList(),
            buildDateTime: buildDateTime
        )
        {
        }

        public BuildContext(
            IDictionary<string, object> variables,
            IDictionary<IBuildStep, IDictionary<string, string>> artifacts,
            IEnumerable<IBuildStep> buildSteps,
            DateTime buildDateTime
        )
        {
            this.variables = variables;
            this.artifacts = artifacts;
            BuildDateTime = buildDateTime;
            this.buildSteps = buildSteps.ToList();
        }

        public IEnumerable<string> ReplaceVariables(IEnumerable<string> templates)
        {
            return templates.ReplaceVariables(GetVariableValue);

            string GetVariableValue(string variableName)
            {
                return GetVariable<object>(variableName).ToString();
            }
        }

        public string ReplaceVariables(string template)
        {
            return template.ReplaceVariables(GetVariableValue);

            string GetVariableValue(string variableName)
            {
                return GetVariable<object>(variableName).ToString();
            }
        }

        public bool TryGetBuildStep(string buildStepName, out IBuildStep buildStep)
        {
            if (string.IsNullOrWhiteSpace(buildStepName))
            {
                throw new ArgumentException(
                    $"{nameof(buildStepName)} cannot be blank or null",
                    nameof(buildStepName)
                );
            }

            foreach (var otherBuildStep in buildSteps)
            {
                if (string.Equals(otherBuildStep.Name, buildStepName))
                {
                    buildStep = otherBuildStep;
                    return true;
                }
            }

            buildStep = default;
            return false;
        }

        public T GetVariable<T>(string variableName)
        {
            if (TryGetVariable<T>(variableName, out var value) == false)
            {
                throw new Exception($"Variable {variableName} is not added to build context");
            }

            return value;
        }

        public bool TryGetVariable<T>(string variableName, out T value)
        {
            if (string.IsNullOrWhiteSpace(variableName))
            {
                throw new ArgumentException(
                    $"{nameof(variableName)} cannot be blank or null",
                    nameof(variableName)
                );
            }

            var normalizedName = GetNormalizedName(variableName);
            if (variables.TryGetValue(normalizedName, out var rawValue) == false)
            {
                value = default;
                return false;
            }

            if (rawValue is not T typedValue)
            {
                value = default;
                return false;
            }

            value = typedValue;
            return true;
        }

        public IEnumerable<string> GetArtifactPaths(IBuildStep buildStep)
        {
            if (buildStep == default)
            {
                throw new ArgumentException(
                    $"{nameof(buildStep)} cannot be null",
                    nameof(buildStep)
                );
            }

            if (artifacts.TryGetValue(buildStep, out var buildStepArtifacts))
            {
                return buildStepArtifacts.Values;
            }

            return Array.Empty<string>();
        }

        public void AddVariable(string variableName, object variableValue)
        {
            if (string.IsNullOrWhiteSpace(variableName))
            {
                throw new ArgumentException(
                    $"{nameof(variableName)} cannot be blank or null",
                    nameof(variableName)
                );
            }

            if (variableValue == default)
            {
                throw new ArgumentException(
                    $"{nameof(variableValue)} cannot be null",
                    nameof(variableValue)
                );
            }

            var normalizedName = GetNormalizedName(variableName);
            variables[normalizedName] = variableValue;
        }

        public void AddArtifact(IBuildStep buildStep, string artifactPath)
        {
            AddArtifact(buildStep, Guid.NewGuid().ToString(), artifactPath);
        }

        public void AddArtifact(IBuildStep buildStep, string artifactName, string artifactPath)
        {
            if (string.IsNullOrWhiteSpace(artifactName))
            {
                throw new ArgumentException(
                    $"{nameof(artifactName)} cannot be blank or null",
                    nameof(artifactName)
                );
            }

            if (string.IsNullOrWhiteSpace(artifactPath))
            {
                throw new ArgumentException(
                    $"{nameof(artifactPath)} cannot be blank or null",
                    nameof(artifactPath)
                );
            }

            var normalizedName = GetNormalizedName(artifactName);
            var normalizedPath = GetNormalizedPath(artifactPath);

            if (artifacts.TryGetValue(buildStep, out var buildStepArtifacts))
            {
                buildStepArtifacts[normalizedName] = normalizedPath;
            }
            else
            {
                artifacts[buildStep] = new Dictionary<string, string>
                {
                    { normalizedName, normalizedPath },
                };
            }
        }

        private static string GetNormalizedName(string name)
        {
            return name.Trim().ToLower();
        }

        private static string GetNormalizedPath(string path)
        {
            return path.Trim();
        }

        public object Clone()
        {
            var context = new BuildContext(
                variables: new Dictionary<string, object>(variables),
                artifacts: new Dictionary<IBuildStep, IDictionary<string, string>>(artifacts),
                buildSteps: buildSteps,
                buildDateTime: BuildDateTime
            );

            return context;
        }
    }
}
