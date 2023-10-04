using UnityEngine.UIElements;

namespace CHARK.BuildTools.Editor.Elements
{
    internal sealed class BuildConfigurationFoldout : Foldout
    {
        internal BuildConfigurationFoldout()
        {
            value = false;
        }

        internal void AddHeader(VisualElement element)
        {
            var toggle = this.Q<Toggle>();
            toggle.Add(element);
        }

        internal void AddContent(VisualElement element)
        {
            Add(element);
        }

        internal void Bind(BuildConfiguration configuration)
        {
            tooltip = configuration.Name;
            text = configuration.Name;
        }
    }
}
