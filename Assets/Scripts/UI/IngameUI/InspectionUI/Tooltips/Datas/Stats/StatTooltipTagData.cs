using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "StatTooltipTagData", menuName = "Tooltips/Stats/TagData")]

public sealed class StatTooltipTagData : TooltipTagData
{
    [SerializeField] private string _associatedStatTypeName;

    [Header("UI")] 
    [SerializeField] private bool _presentAsMultiplier;
    [SerializeField] private bool _lowValueIsGood;

    [Header("BaseDependencyStat")] 
    [SerializeField] private bool _dependsOnStat;
    [ShowIf("_dependsOnStat")] [SerializeField] private StatTooltipTagData _dependencyStatData;
    [ShowIf("_dependsOnStat")] [SerializeField] private string _resultStatLocKey;
    
    public string AssociatedStatTypeName => _associatedStatTypeName;
    public bool PresentAsMultiplier => _presentAsMultiplier;
    public bool LowValueIsGood => _lowValueIsGood;
    public bool DependsOnStat => _dependsOnStat;
    public StatTooltipTagData DependencyStatData => _dependencyStatData;
    public string ResultStatLocKey => _resultStatLocKey;
}