using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace CHARK.BuildTools.Editor.Elements
{
    internal sealed class BuildConfigurationList : ListView
    {
        private readonly List<BuildConfiguration> configurations;

        internal event Action<BuildConfiguration> OnBuildButtonClicked;

        internal event Action OnSortOrderChanged;

        internal BuildConfigurationList(List<BuildConfiguration> configurations)
        {
            this.configurations = configurations;

            showAlternatingRowBackgrounds = AlternatingRowBackground.All;
            virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
            selectionType = SelectionType.None;
            reorderMode = ListViewReorderMode.Animated;
            reorderable = true;

            itemsSource = configurations;
            makeItem = MakeItem;
            bindItem = BindItem;

            itemIndexChanged += OnItemSortOrderChanged;
        }

        private BuildConfigurationListItem MakeItem()
        {
            var item = new BuildConfigurationListItem();

            item.OnBuildButtonClicked += OnItemBuildButtonClicked;

            return item;
        }

        private void BindItem(VisualElement element, int index)
        {
            var collection = configurations[index];
            var item = (BuildConfigurationListItem)element;

            item.Bind(collection);
        }

        private void OnItemSortOrderChanged(int srcIndex, int dstIndex)
        {
            OnSortOrderChanged?.Invoke();
        }

        private void OnItemBuildButtonClicked(BuildConfiguration configuration)
        {
            OnBuildButtonClicked?.Invoke(configuration);
        }
    }
}
