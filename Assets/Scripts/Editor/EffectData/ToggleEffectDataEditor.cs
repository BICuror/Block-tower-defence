#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(ToggleEffectData))]

public class ToggleEffectDataEditor : EffectDataCustomEditor<ToggleEffect> {}

#endif