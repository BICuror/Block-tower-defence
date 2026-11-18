#if UNITY_EDITOR
using System.Collections.Generic;
using Combat.Entity.Modificators;
using UnityEditor;

[CustomEditor(typeof(EntityModificatorConfig), true)]

public class EntityModificatorConfigEditor : CustomTypeDropdownEditor<EntityModificator>
{
    protected override void ApplyDropdownItems(List<string> items)
    {
        EntityModificatorConfig modificatorConfig = (EntityModificatorConfig)target;

        modificatorConfig.SetItemTypeNames(items);
    }
}
#endif