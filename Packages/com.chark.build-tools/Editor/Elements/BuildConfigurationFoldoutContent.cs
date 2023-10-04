using UnityEditor.Search;
using UnityEngine.UIElements;

namespace CHARK.BuildTools.Editor.Elements
{
    internal sealed class BuildConfigurationFoldoutContent : VisualElement
    {
        private const string ReadonlyUssClassName = "readonly";

        private ObjectField objectField;

        internal BuildConfigurationFoldoutContent()
        {
            InitializeObjectField();
        }

        internal void Bind(BuildConfiguration configuration)
        {
            objectField.value = configuration;
        }

        private void InitializeObjectField()
        {
            objectField = new ObjectField
            {
                label = "Build Configuration",
            };

            objectField.AddToClassList(ReadonlyUssClassName);

            Add(objectField);
        }
    }
}
