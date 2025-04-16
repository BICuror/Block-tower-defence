#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(EntityModificatorData))]

public class EntityModificatorDataEditor : CustomTypeDropdownEditor<EntityModificatior>
{
    protected override void ApplyDropdownItems(List<string> items)
    {
        EntityModificatorData modificatorData = (EntityModificatorData)target;

        modificatorData.AllEffectTypeNames = items;
    }
}
#endif