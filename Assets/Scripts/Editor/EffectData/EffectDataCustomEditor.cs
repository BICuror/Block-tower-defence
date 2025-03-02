using System.Collections.Generic;

#if UNITY_EDITOR
public abstract class EffectDataCustomEditor<T> : CustomTypeDropdownEditor<T>
{
    protected override void ApplyDropdownItems(List<string> items)
    {
        EffectData effectData = (EffectData)target;

        effectData.AllEffectTypeNames = items;
    }
}
#endif