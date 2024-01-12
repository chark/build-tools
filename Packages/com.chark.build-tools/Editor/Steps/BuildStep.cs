using System;
using System.Collections.Generic;
using System.Linq;
using CHARK.BuildTools.Editor.Contexts;
using CHARK.BuildTools.Editor.Utilities;
using UnityEngine;

namespace CHARK.BuildTools.Editor.Steps
{
    /// <summary>
    /// Generic build step.
    /// </summary>
    [Serializable]
    public abstract class BuildStep : IBuildStep
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Build Step", Expanded = true)]
        [Sirenix.OdinInspector.Required]
#else
        [Header("Build Step")]
#endif
        [SerializeField]
        private string name = Guid.NewGuid().ToString();

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
        protected virtual IEnumerable<string> ConsumesVariables { get; } = Array.Empty<string>();

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

        public IEnumerable<IBuildArtifact> Artifacts => Context.Artifacts;

        public DateTime BuildDateTime => Context.BuildDateTime;

        private IBuildContext currentContext = DefaultBuildContext.Instance;

        /// <summary>
        /// Context currently used by this build step. Never <c>null</c>.
        /// </summary>
        internal IBuildContext Context
        {
            get => currentContext ?? DefaultBuildContext.Instance;
            set => currentContext = value ?? DefaultBuildContext.Instance;
        }

        /// <summary>
        /// Initialize this build step with a new build context.
        /// </summary>
        /// <param name="buildContext"></param>
        internal void Initialize(IBuildContext buildContext)
        {
            Context = buildContext;
        }

        /// <summary>
        /// Dispose this build step.
        /// </summary>
        internal void Dispose()
        {
            Context = default;
        }

        /// <summary>
        /// Execute this build step.
        /// </summary>
        internal void Execute()
        {
            OnExecuted();
        }

        public IEnumerable<string> ReplaceVariables(IEnumerable<string> templates)
        {
            return Context.ReplaceVariables(this, templates);
        }

        public string ReplaceVariables(string template)
        {
            return Context.ReplaceVariables(this, template);
        }

        /// <summary>
        /// Execute this build step.
        /// </summary>
        protected abstract void OnExecuted();

        /// <returns>
        /// Enumerable of build steps retrieved using given <paramref name="buildStepNames"/>.
        /// </returns>
        protected IEnumerable<IBuildStep> GetBuildSteps(IEnumerable<string> buildStepNames)
        {
            foreach (var buildStepName in buildStepNames)
            {
                if (Context.TryGetBuildStep(buildStepName, out var buildStep))
                {
                    yield return buildStep;
                }
            }
        }

        /// <summary>
        /// Add a new variable with <paramref name="variableValue"/> and
        /// <paramref name="variableName"/> to this build step.
        /// </summary>
        protected void AddVariable(string variableName, object variableValue)
        {
            Context.AddVariable(this, variableName, variableValue);
        }

        /// <summary>
        /// Add <paramref name="artifactPath"/> with a randomized name to this step.
        /// </summary>
        protected void AddArtifact(string artifactPath)
        {
            Context.AddArtifact(this, artifactPath);
        }

        /// <summary>
        /// Add <paramref name="artifactPath"/> with <paramref name="artifactName"/> to this
        /// build step.
        /// </summary>
        protected void AddArtifact(string artifactName, string artifactPath)
        {
            Context.AddArtifact(this, artifactName, artifactPath);
        }

        /// <returns>
        /// Enumerable of artifacts from build steps with given <paramref name="buildStepNames"/>.
        /// </returns>
        protected IEnumerable<IBuildArtifact> GetArtifacts(IEnumerable<string> buildStepNames)
        {
            return buildStepNames.SelectMany(GetArtifacts).ToList();
        }

        /// <returns>
        /// Enumerable of artifacts from build step with given <paramref name="buildStepName"/>.
        /// </returns>
        protected IEnumerable<IBuildArtifact> GetArtifacts(string buildStepName)
        {
            return Context.GetArtifacts(buildStepName);
        }

        /// <returns>
        /// Enumerable of variable names retrieved from given <paramref name="templates"/>.
        /// </returns>
        protected IEnumerable<string> GetVariableNames(IEnumerable<string> templates, bool isNormalize = true)
        {
            return templates.GetVariableNames(isNormalize: isNormalize);
        }

        /// <returns>
        /// Enumerable of variable names extracted from given <paramref name="template"/>.
        /// </returns>
        protected IEnumerable<string> GetVariableNames(string template, bool isNormalize = true)
        {
            return template.GetVariableNames(isNormalize: isNormalize);
        }
    }
}
