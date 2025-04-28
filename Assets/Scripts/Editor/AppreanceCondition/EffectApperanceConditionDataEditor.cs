using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR

[CustomEditor(typeof(EffectAppearanceConditionData))]
public sealed class EffectApperanceConditionDataEditor : CustomTypeDropdownEditor<EffectApperanceCondition>
{
    protected override void ApplyDropdownItems(List<string> items)
    {
        EffectAppearanceConditionData appearanceConditionData = (EffectAppearanceConditionData)target;

        appearanceConditionData.AllConditioinTypeNames = items;
    }
}
#endif