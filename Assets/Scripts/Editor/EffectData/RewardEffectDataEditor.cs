#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(RewardGlobalEffectData))]

public class RewardEffectDataEditor : CustomTypeDropdownEditor<GlobalRewardEffect>
{
    protected override void ApplyDropdownItems(List<string> items)
    {
        RewardGlobalEffectData modificatorData = (RewardGlobalEffectData)target;

        modificatorData.SetItemTypeNames(items);
    }
}

#endif