using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CHARK.BuildTools.Editor.Utilities
{
    internal static class PathUtilities
    {
        internal static string GetFullPath(this string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return string.Empty;
            }

            return Path.GetFullPath(path);
        }

        internal static string GetTopMostDirectory(this IEnumerable<string> paths)
        {
            var pathList = paths?.ToList();
            pathList.AssertNotNull(nameof(paths));
            pathList.AssertNotEmpty(nameof(paths));

            // ReSharper disable once AssignNullToNotNullAttribute
            var commonRoot = Path.GetFullPath(pathList.First());

            foreach (var path in pathList.Skip(1))
            {
                commonRoot = GetCommonRoot(commonRoot, Path.GetFullPath(path));
            }

            return commonRoot;
        }

        private static string GetCommonRoot(string path1, string path2)
        {
            while (string.IsNullOrEmpty(path1) == false && path2.StartsWith(path1, StringComparison.OrdinalIgnoreCase) == false)
            {
                path1 = Path.GetDirectoryName(path1);
            }

            return path1;
        }
    }
}
