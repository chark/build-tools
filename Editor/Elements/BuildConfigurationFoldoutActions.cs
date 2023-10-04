using System;
using UnityEngine.UIElements;

namespace CHARK.BuildTools.Editor.Elements
{
    internal sealed class BuildConfigurationFoldoutActions : VisualElement
    {
        private Button buildButton;

        internal event Action OnBuildButtonClicked;

        internal BuildConfigurationFoldoutActions()
        {
            InitializeBuildButton();
        }

        internal void Bind(BuildConfiguration configuration)
        {
            buildButton.tooltip = $"Build configuration \"{configuration.Name}\"";
        }

        private void InitializeBuildButton()
        {
            buildButton = new Button
            {
                text = "Build",
                tooltip = "Build this configuration",
            };

            buildButton.clicked += OnActionsBuildButtonClicked;

            Add(buildButton);
        }

        private void OnActionsBuildButtonClicked()
        {
            OnBuildButtonClicked?.Invoke();
        }
    }
}
