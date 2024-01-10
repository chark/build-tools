#if ODIN_INSPECTOR

using Unity.SharpZipLib.Zip.Compression;

namespace CHARK.BuildTools.Editor.Drawers
{
    // ReSharper disable once UnusedType.Global
    internal sealed class CompressionLevelEnumValueDrawer : HumanReadableEnumValueDrawer<Deflater.CompressionLevel>
    {
    }
}

#endif
