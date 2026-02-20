#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(GlobalEffectData), true)]

public class ToggleEffectDataEditor : CustomTypeDropdownEditor<GlobalEffect>
{
    protected override void ApplyDropdownItems(List<string> items)
    {
        GlobalEffectData modificatorData = (GlobalEffectData)target;

        modificatorData.SetItemTypeNames(items);
    }
}

#endif