using System;
using System.Collections.Generic;
using System.Linq;
using CHARK.BuildTools.Editor.Context;
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
        [Sirenix.OdinInspector.FoldoutGroup("General", Expanded = true)]
        [Sirenix.OdinInspector.ListDrawerSettings(DefaultExpandedState = true, ShowPaging = false)]
#endif
        [SerializeField]
        private List<BuildStep> steps = new();

        /// <summary>
        /// Enumerable of valid build steps.
        /// </summary>
        public IEnumerable<BuildStep> Steps => steps.Select(step => step);

        /// <summary>
        /// Count of valid build steps.
        /// </summary>
        public int BuildStepCount => Steps.Count();

        /// <summary>
        /// Build this configuration.
        /// </summary>
        public void Build()
        {
            var context = new BuildContext(DateTime.Now);
            foreach (var step in Steps)
            {
                Logging.LogDebug($"Executing step: {step.Name}", this);
                step.ExecuteInternal(context);
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
            if (steps.Count <= 0)
            {
                return name;
            }

            var stepNames = steps.Select(step => step.Name);
            var stepStr = string.Join(", ", stepNames);

            return $"{name}: {stepStr}";
        }
    }
}
