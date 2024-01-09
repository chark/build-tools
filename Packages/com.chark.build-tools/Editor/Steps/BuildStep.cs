using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Variables required by this step.
        /// </summary>
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Debug")]
        [Sirenix.OdinInspector.ListDrawerSettings(
            ShowPaging = false,
            IsReadOnly = true,
            ShowFoldout = false
        )]
        [Sirenix.OdinInspector.ShowInInspector]
#endif
        protected virtual IEnumerable<string> RequiresVariables { get; } = Array.Empty<string>();

        /// <summary>
        /// Variables produced by this step.
        /// </summary>
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Debug")]
        [Sirenix.OdinInspector.ListDrawerSettings(
            ShowPaging = false,
            IsReadOnly = true,
            ShowFoldout = false
        )]
        [Sirenix.OdinInspector.ShowInInspector]
#endif
        protected virtual IEnumerable<string> ProducesVariables { get; } = Array.Empty<string>();

        /// <summary>
        /// Wrapper for <see cref="Execute"/>.
        /// </summary>
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
