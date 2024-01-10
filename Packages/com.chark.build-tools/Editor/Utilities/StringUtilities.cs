using System.Globalization;

namespace CHARK.BuildTools.Editor.Utilities
{
    internal static class StringUtilities
    {
        /// <returns>
        /// <paramref name="value"/> converted to human readable string.
        /// </returns>
        public static string ToHumanReadableString<T>(this T value)
        {
            return value?.ToString().ToHumanReadableString();
        }

        /// <returns>
        /// <paramref name="value"/> converted to human readable string.
        /// </returns>
        public static string ToHumanReadableString(this string value)
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
