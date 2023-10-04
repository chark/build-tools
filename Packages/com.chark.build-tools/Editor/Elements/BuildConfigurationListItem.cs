using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace CHARK.BuildTools.Editor.Elements
{
    internal sealed class BuildConfigurationListItem : VisualElement
    {
        private BuildConfiguration configuration;

        private BuildConfigurationFoldout foldout;
        private BuildConfigurationFoldoutActions actions;
        private BuildConfigurationFoldoutContent content;

        internal event Action<BuildConfiguration> OnBuildButtonClicked;

        internal BuildConfigurationListItem()
        {
            InitializeFoldout();
            InitializeActions();
            InitializeContent();
        }

        internal void Bind(BuildConfiguration newConfiguration)
        {
            var oldCollection = configuration;

            configuration = newConfiguration;

            if (oldCollection != newConfiguration)
            {
                this.Unbind();
                this.TrackSerializedObjectValue(
                    new SerializedObject(configuration),
                    _ => Bind()
                );
            }

            Bind();
        }

        private void Bind()
        {
            foldout.Bind(configuration);
            actions.Bind(configuration);
            content.Bind(configuration);
        }

        private void InitializeFoldout()
        {
            foldout = new BuildConfigurationFoldout();
            Add(foldout);
        }

        private void InitializeActions()
        {
            actions = new BuildConfigurationFoldoutActions();
            actions.OnBuildButtonClicked += OnActionsBuildButtonClicked;

            foldout.AddHeader(actions);
        }

        private void InitializeContent()
        {
            content = new BuildConfigurationFoldoutContent();
            foldout.AddContent(content);
        }

        private void OnActionsBuildButtonClicked()
        {
            OnBuildButtonClicked?.Invoke(configuration);
        }
    }
}
