#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(ToggleGlobalEffectData), true)]

public class ToggleEffectDataEditor : CustomTypeDropdownEditor<GlobalToggleEffect>
{
    protected override void ApplyDropdownItems(List<string> items)
    {
        ToggleGlobalEffectData modificatorData = (ToggleGlobalEffectData)target;

        modificatorData.SetItemTypeNames(items);
    }
}

#endif