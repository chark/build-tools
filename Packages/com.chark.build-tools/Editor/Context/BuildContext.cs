using System;
using System.Collections.Generic;
using CHARK.BuildTools.Editor.Utilities;

namespace CHARK.BuildTools.Editor.Context
{
    internal sealed class BuildContext : IBuildContext
    {
        private readonly IDictionary<string, IBuildContext.VariableValueProvider> valueProviders =
            new Dictionary<string, IBuildContext.VariableValueProvider>();

        private readonly IDictionary<string, string> artifacts =
            new Dictionary<string, string>();

        public DateTime BuildDateTime { get; }

        public IEnumerable<string> ArtifactPaths => artifacts.Values;

        public BuildContext(DateTime buildDateTime)
        {
            BuildDateTime = buildDateTime;
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

        public string GetArtifactPath(string artifactName)
        {
            if (TryGetArtifactPath(artifactName, out var value) == false)
            {
                throw new Exception($"Artifact {artifactName} is not added to build context");
            }

            return value;
        }

        public bool TryGetArtifactPath(string artifactName, out string artifactPath)
        {
            if (string.IsNullOrWhiteSpace(artifactName))
            {
                throw new ArgumentException("Value cannot be blank or null", nameof(artifactName));
            }

            var normalizedName = GetNormalizedName(artifactName);
            return artifacts.TryGetValue(normalizedName, out artifactPath);
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
                throw new ArgumentException("Value cannot be blank or null", nameof(variableName));
            }

            var normalizedName = GetNormalizedName(variableName);
            if (valueProviders.TryGetValue(normalizedName, out var valueProvider) == false)
            {
                value = default;
                return false;
            }

            var rawValue = valueProvider();
            if (rawValue is not T typedValue)
            {
                value = default;
                return false;
            }

            value = typedValue;
            return true;
        }

        public void AddVariable(string variableName, IBuildContext.VariableValueProvider valueProvider)
        {
            if (string.IsNullOrWhiteSpace(variableName))
            {
                throw new ArgumentException("Value cannot be blank or null", nameof(variableName));
            }

            var normalizedName = GetNormalizedName(variableName);
            valueProviders[normalizedName] = valueProvider;
        }

        public void AddArtifact(string artifactPath)
        {
            AddArtifact(Guid.NewGuid().ToString(), artifactPath);
        }

        public void AddArtifact(string artifactName, string artifactPath)
        {
            if (string.IsNullOrWhiteSpace(artifactName))
            {
                throw new ArgumentException("Value cannot be blank or null", nameof(artifactName));
            }

            if (string.IsNullOrWhiteSpace(artifactPath))
            {
                throw new ArgumentException("Value cannot be blank or null", nameof(artifactPath));
            }

            var normalizedName = GetNormalizedName(artifactName);
            var normalizedPath = GetNormalizedPath(artifactPath);

            artifacts[normalizedName] = normalizedPath;
        }

        private static string GetNormalizedName(string name)
        {
            return name.Trim().ToLower();
        }

        private static string GetNormalizedPath(string path)
        {
            return path.Trim();
        }
    }
}
