#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(RewardEffectData))]

public class RewardEffectDataEditor : EffectDataCustomEditor<RewardEffect> {}

#endif