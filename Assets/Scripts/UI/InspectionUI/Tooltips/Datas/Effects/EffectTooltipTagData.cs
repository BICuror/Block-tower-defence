using UnityEngine;

[CreateAssetMenu(fileName = "EffectTooltipTagData", menuName = "Tooltips/Effects/TagData")]

public sealed class EffectTooltipTagData : TooltipTagData
{
    [SerializeField] private string _associatedEffectTypeName;
    
    public string AssociatedEffectTypeName => _associatedEffectTypeName;
}