using CHARK.BuildTools.Editor.Context;
using UnityEngine;

namespace CHARK.BuildTools.Editor.Steps
{
    /// <summary>
    /// Generic build step.
    /// </summary>
    public abstract class BuildStep : ScriptableObject
    {
        /// <summary>
        /// Name of this build step.
        /// </summary>
        public string Name => name;

        internal void ExecuteInternal(IBuildContext context)
        {
            Execute(context);
        }

        /// <summary>
        /// Execute this build step using provided <paramref name="context"/>.
        /// </summary>
        protected abstract void Execute(IBuildContext context);
    }
}
