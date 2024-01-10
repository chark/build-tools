#if ODIN_INSPECTOR

using System;
using System.Linq;
using CHARK.BuildTools.Editor.Utilities;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEngine;

namespace CHARK.BuildTools.Editor.Drawers
{
    internal abstract class HumanReadableEnumValueDrawer<T> : OdinValueDrawer<T> where T : Enum
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            DrawSelectorDropdown(label);
        }

        private void DrawSelectorDropdown(GUIContent label)
        {
            var values = EnumSelector<T>.DrawSelectorDropdown(label, GetEnumDropdownName(), ShowSelector);
            if (values == null)
            {
                return;
            }

            ValueEntry.SmartValue = values.FirstOrDefault();
        }

        private GUIContent GetEnumDropdownName()
        {
            var name = EnumTypeUtilities<T>.IsFlagEnum ? GetFlagEnumName() : GetEnumName();
            return new GUIContent(name);
        }

        private string GetFlagEnumName()
        {
            var currentEnumValue = ValueEntry.SmartValue;
            var flagValues = EnumTypeUtilities<T>.DecomposeEnumFlagValues(currentEnumValue);
            if (flagValues.Length == 0)
            {
                return currentEnumValue.ToHumanReadableString();
            }

            var flagNames = flagValues
                .Select(EnumTypeUtilities<T>.GetEnumMemberInfo)
                .Select(info => info.Name.ToHumanReadableString());

            return string.Join(", ", flagNames);
        }

        private string GetEnumName()
        {
            var memberInfo = EnumTypeUtilities<T>.GetEnumMemberInfo(ValueEntry.SmartValue);
            return memberInfo.Name.ToHumanReadableString();
        }

        private EnumSelector<T> ShowSelector(Rect rect)
        {
            var selector = CreateSelector();
            selector.ShowInPopup(rect);

            return selector;
        }

        private EnumSelector<T> CreateSelector()
        {
            var selector = new EnumSelector<T>();

            var selectionTree = selector.SelectionTree;
            var menuItems = selectionTree.MenuItems;

            for (var index = menuItems.Count - 1; index >= 0; index--)
            {
                var menuItem = menuItems[index];

                var enumMember = (EnumTypeUtilities<T>.EnumMember)menuItem.Value;
                var enumName = enumMember.Name;

                if (enumMember.IsObsolete)
                {
                    menuItem.Remove();
                    continue;
                }

                menuItem.Name = enumName.ToHumanReadableString();
            }

            selector.SetSelection(ValueEntry.SmartValue);

            return selector;
        }
    }
}

#endif
