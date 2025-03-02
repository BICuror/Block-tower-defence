#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(EntityEffectData))]

public class EntityEffectDataEditor : EffectDataCustomEditor<EntityEffect> {}

#endif