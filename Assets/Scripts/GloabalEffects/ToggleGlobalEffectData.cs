using UnityEngine;

[CreateAssetMenu(fileName = "ToggleGlobalEffectData", menuName = "Effects/ToggleGlobalEffectData")]

public class ToggleGlobalEffectData : GlobalEffectData
{
    public virtual void Modify(GlobalToggleEffect effect) {}
}