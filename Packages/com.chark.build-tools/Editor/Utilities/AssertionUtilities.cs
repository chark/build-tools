using System;
using System.Collections.Generic;
using System.Linq;

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

        internal static void AssertNotEmpty<T>(this IEnumerable<T> enumerable, string valueName)
        {
            if (enumerable.Any())
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
