using UnityEngine;

[CreateAssetMenu(fileName = "StatTooltipTagData", menuName = "Tooltips/Stats/TagData")]

public sealed class StatTooltipTagData : TooltipTagData
{   
    [SerializeField] private string _associatedStatTypeName;
    
    public string AssociatedStatTypeName => _associatedStatTypeName;
}