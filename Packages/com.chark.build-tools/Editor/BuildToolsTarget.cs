using UnityEditor;

namespace CHARK.BuildTools.Editor
{
    /// <summary>
    /// Target platform, used instead of <see cref="BuildTarget"/> in order to restrict supported
    /// platforms.
    /// </summary>
    internal enum BuildToolsTarget
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.LabelText("StandaloneWindows")]
#endif
        StandaloneWindows,

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.LabelText("StandaloneWindows64")]
#endif
        StandaloneWindows64,

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.LabelText("Android")]
#endif
        Android,

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.LabelText("StandaloneLinux64")]
#endif
        StandaloneLinux64,

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.LabelText("StandaloneOSX")]
#endif
        StandaloneOSX,

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.LabelText("iOS")]
#endif
        // ReSharper disable once InconsistentNaming
        iOS,

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.LabelText("WebGL")]
#endif
        WebGL,
    }
}
