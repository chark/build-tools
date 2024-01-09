using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CHARK.BuildTools.Editor.Context
{
    internal sealed class BuildContext : IBuildContext
    {
        private readonly IDictionary<string, IBuildContext.VariableValueProvider> valueProviders =
            new Dictionary<string, IBuildContext.VariableValueProvider>();

        private readonly IDictionary<string, string> artifacts =
            new Dictionary<string, string>();

        public DateTime BuildDateTime { get; }

        public IEnumerable<string> Artifacts => artifacts.Values;

        public BuildContext(DateTime buildDateTime)
        {
            BuildDateTime = buildDateTime;
        }

        public string ReplaceVariables(string template)
        {
            if (string.IsNullOrWhiteSpace(template))
            {
                throw new ArgumentException("Value cannot be blank or null", nameof(template));
            }

            var result = template.Trim();
            foreach (var variableName in GetVariableNames(template))
            {
                var value = GetVariable<object>(variableName);

                result = result.Replace(
                    $"{{{variableName}}}",
                    value.ToString(),
                    StringComparison.OrdinalIgnoreCase
                );
            }

            return result;
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

        private static IEnumerable<string> GetVariableNames(string input)
        {
            const string pattern = @"\{([^}]+)\}";

            var matches = Regex.Matches(input, pattern);
            foreach (Match match in matches)
            {
                if (match.Success == false)
                {
                    continue;
                }

                var groups = match.Groups;
                if (groups.Count <= 1)
                {
                    continue;
                }

                var group = groups[1];
                var value = group.Value;
                var variableName = GetNormalizedName(value);

                yield return variableName;
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
    }
}
