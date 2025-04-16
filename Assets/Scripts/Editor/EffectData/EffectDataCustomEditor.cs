using System.Collections.Generic;

#if UNITY_EDITOR
public abstract class EffectDataCustomEditor<T> : CustomTypeDropdownEditor<T>
{
    protected override void ApplyDropdownItems(List<string> items)
    {
        GlobalEffectData globalEffectData = (GlobalEffectData)target;

        globalEffectData.AllEffectTypeNames = items;
    }
}
#endif