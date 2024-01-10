using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CHARK.BuildTools.Editor.Utilities
{
    internal static class StringUtilities
    {
        /// <summary>
        /// Enumerable of variable names extracted from <paramref name="templates"/>.
        /// </summary>
        /// <param name="templates"></param>
        /// <returns></returns>
        internal static IEnumerable<string> GetVariableNames(this IEnumerable<string> templates)
        {
            foreach (var template in templates)
            {
                foreach (var variableName in GetVariableNames(template))
                {
                    yield return variableName;
                }
            }
        }

        /// <returns>
        /// Enumerable of variable names extracted from <paramref name="template"/>.
        /// </returns>
        internal static IEnumerable<string> GetVariableNames(this string template)
        {
            if (string.IsNullOrWhiteSpace(template))
            {
                yield break;
            }

            const string pattern = @"\{([^}]+)\}";

            var matches = Regex.Matches(template, pattern);
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
                var variableName = value.ToNormalizedVariableName();

                yield return variableName;
            }
        }

        /// <returns>
        /// Enumerable of strings where variables inside <paramref name="templates"/> are replaced
        /// with values provided by <paramref name="variableProvider"/>.
        /// </returns>
        internal static IEnumerable<string> ReplaceVariables(
            this IEnumerable<string> templates,
            Func<string, string> variableProvider
        )
        {
            foreach (var template in templates)
            {
                yield return ReplaceVariables(template, variableProvider);
            }
        }

        /// <returns>
        /// New string where variables inside <paramref name="template"/> are replaced with values
        /// provided by <paramref name="variableProvider"/>.
        /// </returns>
        internal static string ReplaceVariables(this string template, Func<string, string> variableProvider)
        {
            if (string.IsNullOrWhiteSpace(template))
            {
                return string.Empty;
            }

            var result = template.Trim();
            foreach (var variableName in GetVariableNames(template))
            {
                var value = variableProvider(variableName);

                result = result.Replace(
                    $"{{{variableName}}}",
                    value,
                    StringComparison.OrdinalIgnoreCase
                );
            }

            return result;
        }

        /// <returns>
        /// <paramref name="name"/> normalized to a value which can be used as a variable name
        /// in interpolation.
        /// </returns>
        internal static string ToNormalizedVariableName(this string name)
        {
            return name == null ? string.Empty : name?.Trim().ToLower();
        }

        /// <returns>
        /// <paramref name="value"/> converted to human readable string.
        /// </returns>
        internal static string ToHumanReadableString<T>(this T value)
        {
            return value == null ? string.Empty : value.ToString().ToHumanReadableString();
        }

        /// <returns>
        /// <paramref name="value"/> converted to human readable string.
        /// </returns>
        internal static string ToHumanReadableString(this string value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            var textInfo = CultureInfo.CurrentCulture.TextInfo;
            var parts = value.Split('_');

            for (var index = 0; index < parts.Length; index++)
            {
                var lowerPart = parts[index].ToLower();
                var title = textInfo.ToTitleCase(lowerPart);

                parts[index] = title;
            }

            return string.Join(" ", parts);
        }
    }
}
