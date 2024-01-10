using System;
using System.Collections.Generic;

namespace CHARK.BuildTools.Editor.Context
{
    public interface IBuildContext
    {
        /// <summary>
        /// Delegate which provides raw values for <see cref="IBuildContext.TryGetVariable{T}"/> and
        /// <see cref="IBuildContext.GetVariable{T}"/>.
        /// </summary>
        public delegate object VariableValueProvider();

        /// <summary>
        /// Date time when this build was started.
        /// </summary>
        public DateTime BuildDateTime { get; }

        /// <summary>
        /// All produced artifact paths.
        /// </summary>
        public IEnumerable<string> ArtifactPaths { get; }

        /// <returns>
        /// String with replaced variables specified in <paramref name="templates"/>.
        /// </returns>
        public IEnumerable<string> ReplaceVariables(IEnumerable<string> templates);

        /// <returns>
        /// String with replaced variables specified in <paramref name="template"/>.
        /// </returns>
        public string ReplaceVariables(string template);

        /// <returns>
        /// Path of artifact with <paramref name="artifactName"/>.
        /// </returns>
        public string GetArtifactPath(string artifactName);

        /// <returns>
        /// <c>true</c> if <paramref name="artifactPath"/> is retrieved by given
        /// <paramref name="artifactName"/> or <c>false</c> otherwise.
        /// </returns>
        public bool TryGetArtifactPath(string artifactName, out string artifactPath);

        /// <returns>
        /// Variable of type <see cref="T"/> retrieved by given <paramref name="variableName"/>.
        /// </returns>
        public T GetVariable<T>(string variableName);

        /// <returns>
        /// <c>true</c> if variable of type <see cref="T"/> is retrieved by given
        /// <paramref name="variableName"/> or <c>false</c> otherwise.
        /// </returns>
        public bool TryGetVariable<T>(string variableName, out T value);

        /// <summary>
        /// Add build variable.
        /// </summary>
        public void AddVariable(string variableName, VariableValueProvider valueProvider);

        /// <summary>
        /// Add built artifact to context.
        /// </summary>
        public void AddArtifact(string artifactPath);

        /// <summary>
        /// Add built artifact to context.
        /// </summary>
        public void AddArtifact(string artifactName, string artifactPath);
    }
}
