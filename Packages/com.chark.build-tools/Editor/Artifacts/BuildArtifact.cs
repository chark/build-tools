using CHARK.BuildTools.Editor.Steps;

namespace CHARK.BuildTools.Editor
{
    internal sealed class BuildArtifact : IBuildArtifact
    {
        public IBuildStep BuildStep { get; }

        public string Name { get; }

        public string Path { get; }

        public BuildArtifact(IBuildStep buildStep, string name, string path)
        {
            BuildStep = buildStep;
            Name = name;
            Path = path;
        }
    }
}
