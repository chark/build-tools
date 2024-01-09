using System.Collections.Generic;
using CHARK.BuildTools.Editor.Elements;
using CHARK.BuildTools.Editor.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CHARK.BuildTools.Editor
{
    /// <summary>
    /// Window used to manage build settings.
    /// </summary>
    internal sealed class BuildToolsManagerWindow : EditorWindow
    {
        [SerializeField]
        private StyleSheet styleSheet;

        private readonly List<BuildConfiguration> buildConfigurations = new();
        private BuildConfigurationList listView;

        [MenuItem(
            MenuItemConstants.BaseWindowItemName + "/Build Tools",
            priority = MenuItemConstants.BaseWindowPriority
        )]
        private static void ShowWindow()
        {
            var sceneManagerWindow = GetWindow<BuildToolsManagerWindow>();
            sceneManagerWindow.titleContent = new GUIContent("Build Tools");

            var minSize = sceneManagerWindow.minSize;
            minSize.x = 100f;
            minSize.y = 100f;
            sceneManagerWindow.minSize = minSize;

            sceneManagerWindow.Show();
        }

        private void OnEnable()
        {
            BuildToolsEditorUtilities.OnEditorStateChanged += OnEditorStateChanged;

            BindUIElements();
            SetupElements();
            SetupStyleSheets();
        }

        private void OnDisable()
        {
            BuildToolsEditorUtilities.OnEditorStateChanged -= OnEditorStateChanged;
        }

        private void SetupElements()
        {
            rootVisualElement.AddToClassList("content");

            listView = new BuildConfigurationList(buildConfigurations);
            listView.OnSortOrderChanged += OnListViewSortOrderChanged;
            listView.OnBuildButtonClicked += OnListViewBuildButtonClicked;

            rootVisualElement.AddToClassList("window-content");
            rootVisualElement.Add(listView);
        }

        private void SetupStyleSheets()
        {
            rootVisualElement.styleSheets.Add(styleSheet);
        }

        private void BindUIElements()
        {
            BindListView();
        }

        private void BindListView()
        {
            buildConfigurations.Clear();
            buildConfigurations.AddRange(BuildToolsEditorUtilities.GetBuildConfigurations());
            listView?.RefreshItems();
        }

        private void OnEditorStateChanged()
        {
            BindUIElements();
        }

        private void OnListViewSortOrderChanged()
        {
            for (var index = 0; index < buildConfigurations.Count; index++)
            {
                var configuration = buildConfigurations[index];
                if (configuration.GetDisplayOrder() == index)
                {
                    continue;
                }

                configuration.SetDisplayOrder(index);
            }
        }

        private static void OnListViewBuildButtonClicked(BuildConfiguration configuration)
        {
            configuration.Build();
        }
    }
}
