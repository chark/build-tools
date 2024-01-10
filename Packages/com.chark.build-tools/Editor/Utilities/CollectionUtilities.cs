using System.Collections.Generic;
using System.Linq;

namespace CHARK.BuildTools.Editor.Utilities
{
    internal static class CollectionUtilities
    {
        /// <returns>
        /// New <see cref="Enumerable"/> with appended <paramref name="appendEnumerable"/> data.
        /// </returns>
        public static IEnumerable<TValue> Append<TValue>(
            this IEnumerable<TValue> enumerable,
            IEnumerable<TValue> appendEnumerable
        )
        {
            var result = enumerable;
            foreach (var value in appendEnumerable)
            {
                result = result.Append(value);
            }

            return result;
        }
    }
}
