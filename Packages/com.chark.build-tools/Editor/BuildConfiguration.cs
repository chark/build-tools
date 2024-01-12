using System;
using System.Collections.Generic;
using System.Linq;
using CHARK.BuildTools.Editor.Contexts;
using CHARK.BuildTools.Editor.Steps;
using CHARK.BuildTools.Editor.Utilities;
using UnityEngine;

// TODO: validate required and produced variables in Editor
// TODO: better logging - what kind of variables are being injected?
namespace CHARK.BuildTools.Editor
{
    /// <summary>
    /// Configuration which represents a build.
    /// </summary>
    [CreateAssetMenu(
        fileName = CreateAssetMenuConstants.BaseFileName + nameof(BuildConfiguration),
        menuName = CreateAssetMenuConstants.BaseMenuName + "/Build Configuration",
        order = CreateAssetMenuConstants.BaseOrder
    )]
    internal sealed class BuildConfiguration : ScriptableObject
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("General", Expanded = true)]
#else
        [Header("General")]
#endif
        [SerializeField]
        private string configurationName;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Steps", Expanded = true)]
        [Sirenix.OdinInspector.ListDrawerSettings(
            DefaultExpandedState = true,
            ShowFoldout = false,
            ShowPaging = false,
            ListElementLabelName = nameof(BuildStep.Name)
        )]
#endif
        [SerializeReference]
        private List<BuildStep> steps = new();

        /// <summary>
        /// Enumerable of valid build steps.
        /// </summary>
        public IEnumerable<BuildStep> Steps => steps.Where(step => step != default);

        /// <summary>
        /// Count of valid build steps.
        /// </summary>
        public int BuildStepCount => Steps.Count();

        private void OnValidate()
        {
            InitializeSteps();
        }

        private void OnEnable()
        {
            InitializeSteps();
        }

        private void OnDisable()
        {
            DisposeBuildSteps();
        }

        /// <summary>
        /// Build this configuration.
        /// </summary>
        public void Build()
        {
            InitializeSteps();

            try
            {
                ExecuteBuildSteps();
            }
            finally
            {
                DisposeBuildSteps();
            }
        }

        /// <summary>
        /// User-friendly configuration name.
        /// </summary>
        public string Name
        {
            get
            {
                if (string.IsNullOrWhiteSpace(configurationName))
                {
                    return GetDefaultName();
                }

                return configurationName;
            }
        }

        private string GetDefaultName()
        {
            if (BuildStepCount <= 0)
            {
                return name;
            }

            var stepNames = Steps.Select(step => step.Name);
            var stepStr = string.Join(", ", stepNames);

            return $"{name}: {stepStr}";
        }

        private void InitializeSteps()
        {
            var context = new BuildContext(DateTime.Now, Steps);
            foreach (var step in Steps)
            {
                step.Initialize(context);
            }
        }

        private void ExecuteBuildSteps()
        {
            foreach (var step in Steps)
            {
                Logging.LogDebug($"Executing build step \"{step.Name}\"", this);
                try
                {
                    step.Execute();
                    Logging.LogDebug($"Build step \"{step.Name}\" succeeded", this);
                }
                catch (Exception exception)
                {
                    Logging.LogError($"Build step \"{step.Name}\" failed due to: {exception.Message}", this);
                    Logging.LogException(exception, this);
                }
            }
        }

        private void DisposeBuildSteps()
        {
            foreach (var step in Steps)
            {
                step.Dispose();
            }
        }
    }
}
