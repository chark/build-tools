using UnityEngine;

namespace CHARK.BuildTools.Editor.Utilities
{
    /// <summary>
    /// Constants for <see cref="CreateAssetMenuAttribute"/>.
    /// </summary>
    internal static class CreateAssetMenuConstants
    {
        /// <summary>
        /// Base file name (prefix).
        /// </summary>
        internal const string BaseFileName = "New ";

        /// <summary>
        /// Base menu name (sub-menu category).
        /// </summary>
        internal const string BaseMenuName = "CHARK/Build Tools";

        /// <summary>
        /// Base menu order.
        /// </summary>
        internal const int BaseOrder = -1000;
    }
}
