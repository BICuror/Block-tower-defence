#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(EntityModificationEffectData))]

public class EntityEffectDataEditor : EffectDataCustomEditor<EntityModificationEffect> {}

#endif