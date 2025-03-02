using System.Collections.Generic;
using Navigation;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR

[CustomEditor(typeof(NavigationAgentData))]
public sealed class NavigationAgentDataEditor : CustomTypeDropdownEditor<NavigationAgentNodePicker>
{
    protected override void ApplyDropdownItems(List<string> items)
    {
        NavigationAgentData agentData = (NavigationAgentData)target;

        agentData.AllNavigationNodePickerTypes = items;
    }
}
#endif