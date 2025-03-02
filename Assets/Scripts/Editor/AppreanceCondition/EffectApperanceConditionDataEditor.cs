using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR

[CustomEditor(typeof(EffectApperanceConditionData))]
public sealed class EffectApperanceConditionDataEditor : CustomTypeDropdownEditor<EffectApperanceCondition>
{
    protected override void ApplyDropdownItems(List<string> items)
    {
        EffectApperanceConditionData apperanceConditionData = (EffectApperanceConditionData)target;

        apperanceConditionData.AllConditioinTypeNames = items;
    }
}
#endif