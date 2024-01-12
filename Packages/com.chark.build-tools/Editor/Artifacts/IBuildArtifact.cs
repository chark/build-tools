using CHARK.BuildTools.Editor.Steps;

namespace CHARK.BuildTools.Editor
{
    public interface IBuildArtifact
    {
        public IBuildStep BuildStep { get; }

        public string Name { get; }

        public string Path { get; }
    }
}
