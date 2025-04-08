#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(EntityModificatorData))]

public class EntityModificatorDataEditor : EffectDataCustomEditor<EntityModificatior> {}

#endif