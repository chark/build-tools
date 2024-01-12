using System;

namespace CHARK.BuildTools.Editor.Utilities
{
    internal static class AssertionUtilities
    {
        internal static void AssertNotBlank(this string value, string valueName)
        {
            if (string.IsNullOrWhiteSpace(value) == false)
            {
                return;
            }

            throw new ArgumentException(
                $"{valueName} cannot be null or blank",
                valueName
            );
        }

        internal static void AssertNotNull(this object value, string valueName)
        {
            if (value != default)
            {
                return;
            }

            throw new ArgumentException(
                $"{valueName} cannot be null",
                valueName
            );
        }
    }
}
