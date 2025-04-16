#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(RewardGlobalEffectData))]

public class RewardEffectDataEditor : EffectDataCustomEditor<GlobalRewardEffect> {}

#endif