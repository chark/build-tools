using System;
using System.Collections.Generic;
using System.Linq;
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

        public IEnumerable<string> BuildStepNames
        {
            get
            {
                if (context == default)
                {
                    return Array.Empty<string>();
                }

                return context
                    .BuildSteps
                    .Where(buildStep => buildStep != this)
                    .Select(buildStep => buildStep.Name);
            }
        }

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

        public IEnumerable<IBuildStep.Artifact> Artifacts
        {
            get
            {
                if (context == default)
                {
                    return Array.Empty<IBuildStep.Artifact>();
                }

                return context.Artifacts;
            }
        }

        public DateTime BuildDateTime => context?.BuildDateTime ?? DateTime.Now;

        private BuildContext context;

        internal void Initialize(BuildContext buildContext)
        {
            context = buildContext;
        }

        public IEnumerable<IBuildStep> GetBuildSteps(IEnumerable<string> buildStepNames)
        {
            if (context == default)
            {
                yield break;
            }

            foreach (var buildStepName in buildStepNames)
            {
                if (context.TryGetBuildStep(buildStepName, out var buildStep))
                {
                    yield return buildStep;
                }
            }
        }

        public IEnumerable<IBuildStep.Artifact> GetArtifacts(IEnumerable<string> buildStepNames)
        {
            return buildStepNames.SelectMany(GetArtifacts).ToList();
        }

        public IEnumerable<IBuildStep.Artifact> GetArtifacts(string buildStepName)
        {
            if (context == default)
            {
                return Array.Empty<IBuildStep.Artifact>();
            }

            if (context.TryGetBuildStep(buildStepName, out var buildStep) == false)
            {
                return Array.Empty<IBuildStep.Artifact>();
            }

            return buildStep.Artifacts;
        }

        public void AddVariable(string variableName, object variableValue)
        {
            context?.AddVariable(this, variableName, variableValue);
        }

        public void AddArtifact(string artifactPath)
        {
            AddArtifact(Guid.NewGuid().ToString(), artifactPath);
        }

        public void AddArtifact(string artifactName, string artifactPath)
        {
            context?.AddArtifact(this, artifactName, artifactPath);
        }

        public IEnumerable<string> ReplaceVariables(IEnumerable<string> templates)
        {
            return context?.ReplaceVariables(this, templates) ?? Array.Empty<string>();
        }

        public string ReplaceVariables(string template)
        {
            if (context == default)
            {
                return template;
            }

            return context?.ReplaceVariables(this, template) ?? template;
        }

        public IEnumerable<string> GetVariableNames(IEnumerable<string> templates, bool isNormalize = true)
        {
            return templates.GetVariableNames(isNormalize: isNormalize);
        }

        public IEnumerable<string> GetVariableNames(string template, bool isNormalize = true)
        {
            return template.GetVariableNames(isNormalize: isNormalize);
        }

        public abstract void Execute();
    }
}
