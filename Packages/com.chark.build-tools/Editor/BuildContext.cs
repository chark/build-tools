using System;
using System.Collections.Generic;
using System.Linq;
using CHARK.BuildTools.Editor.Steps;
using CHARK.BuildTools.Editor.Utilities;

namespace CHARK.BuildTools.Editor
{
    internal sealed class BuildContext : ICloneable
    {
        private readonly IDictionary<IBuildStep, IDictionary<string, object>> variables;
        private readonly IDictionary<IBuildStep, IDictionary<string, string>> artifacts;
        private readonly ICollection<IBuildStep> buildSteps;

        public DateTime BuildDateTime { get; }

        public IEnumerable<IBuildStep.Artifact> Artifacts
        {
            get
            {
                foreach (var (buildStep, buildStepArtifacts) in artifacts)
                {
                    foreach (var (name, path) in buildStepArtifacts)
                    {
                        var artifact = new IBuildStep.Artifact(
                            buildStep: buildStep,
                            name: name,
                            path: path
                        );

                        yield return artifact;
                    }
                }
            }
        }

        public IEnumerable<IBuildStep> BuildSteps => buildSteps;

        public BuildContext(DateTime buildDateTime, IEnumerable<IBuildStep> buildSteps) : this(
            variables: new Dictionary<IBuildStep, IDictionary<string, object>>(),
            artifacts: new Dictionary<IBuildStep, IDictionary<string, string>>(),
            buildSteps: buildSteps.ToList(),
            buildDateTime: buildDateTime
        )
        {
        }

        public BuildContext(
            IDictionary<IBuildStep, IDictionary<string, object>> variables,
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

        public IEnumerable<string> ReplaceVariables(IBuildStep buildStep, IEnumerable<string> templates)
        {
            return templates.ReplaceVariables(GetValue);

            string GetValue(string variableName)
            {
                return GetVariableValue(buildStep, variableName);
            }
        }

        public string ReplaceVariables(IBuildStep buildStep, string template)
        {
            return template.ReplaceVariables(GetValue);

            string GetValue(string variableName)
            {
                return GetVariableValue(buildStep, variableName);
            }
        }

        public bool TryGetBuildStep(string buildStepName, out IBuildStep buildStep)
        {
            buildStepName.AssertNotBlank(nameof(buildStepName));

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

        public bool TryGetVariable<T>(IBuildStep buildStep, string variableName, out T value)
        {
            buildStep.AssertNotNull(nameof(buildStep));
            variableName.AssertNotBlank(nameof(variableName));

            if (variables.TryGetValue(buildStep, out var buildStepVariables) == false)
            {
                value = default;
                return false;
            }

            var normalizedName = GetNormalizedName(variableName);
            if (buildStepVariables.TryGetValue(normalizedName, out var rawValue) == false)
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

        public void AddVariable(IBuildStep buildStep, string variableName, object variableValue)
        {
            buildStep.AssertNotNull(nameof(buildStep));
            variableName.AssertNotBlank(nameof(variableName));
            variableValue.AssertNotNull(nameof(variableValue));

            var normalizedName = GetNormalizedName(variableName);
            if (variables.TryGetValue(buildStep, out var buildStepVariables))
            {
                buildStepVariables[normalizedName] = variableValue;
            }
            else
            {
                variables[buildStep] = new Dictionary<string, object>
                {
                    { normalizedName, variableValue },
                };
            }
        }

        public IEnumerable<string> GetArtifactPaths(IBuildStep buildStep)
        {
            buildStep.AssertNotNull(nameof(buildStep));

            if (artifacts.TryGetValue(buildStep, out var buildStepArtifacts))
            {
                return buildStepArtifacts.Values;
            }

            return Array.Empty<string>();
        }

        public void AddArtifact(IBuildStep buildStep, string artifactPath)
        {
            AddArtifact(buildStep, Guid.NewGuid().ToString(), artifactPath);
        }

        public void AddArtifact(IBuildStep buildStep, string artifactName, string artifactPath)
        {
            buildStep.AssertNotNull(nameof(buildStep));
            artifactName.AssertNotBlank(nameof(artifactName));
            artifactPath.AssertNotBlank(nameof(artifactPath));

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

        private string GetVariableValue(IBuildStep buildStep, string variableName)
        {
            if (TryGetVariable<object>(buildStep, variableName, out var value))
            {
                return value.ToString();
            }

            foreach (var otherBuildStep in buildSteps)
            {
                if (otherBuildStep == buildStep)
                {
                    continue;
                }

                if (TryGetVariable<object>(buildStep, variableName, out var otherValue))
                {
                    return otherValue.ToString();
                }
            }

            return string.Empty;
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
                variables: new Dictionary<IBuildStep, IDictionary<string, object>>(variables),
                artifacts: new Dictionary<IBuildStep, IDictionary<string, string>>(artifacts),
                buildSteps: buildSteps,
                buildDateTime: BuildDateTime
            );

            return context;
        }
    }
}
