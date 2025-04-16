#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(ToggleGlobalEffectData))]

public class ToggleEffectDataEditor : EffectDataCustomEditor<GlobalToggleEffect> {}

#endif