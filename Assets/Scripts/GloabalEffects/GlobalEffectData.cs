using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public abstract class GlobalEffectData : ScriptableObject
{
    [Header("GlobalEffectData")]
    [Range(1, 5)] [SerializeField] private int _quality = 3;
    [SerializeField] private EffectType _effectType;
    
    [SerializeField] private bool _isUnique = false;
    [SerializeField] private List<InstanceItemTypeContainer> _instanceItemTypeContainers;
    
    [Header("EffectAppearanceCondition")]
    [SerializeField] private EffectAppearanceConditionData effectAppearanceCondition;
    [ShowIf("HasAppearanceCondition", true)] [SerializeField] private ArgumentsContainer _argumentsContainer;

    [Header("TooltipData")] 
    [TextArea] [SerializeField] private string _effectDescription;
    
    public List<InstanceItemTypeContainer> InstanceItemTypeContainers => _instanceItemTypeContainers;
    public ArgumentsContainer ArgumentsContainer => _argumentsContainer;
    public EffectAppearanceConditionData EffectAppearanceCondition => effectAppearanceCondition;
    public bool HasAppearanceCondition => effectAppearanceCondition;
    public int Quality => _quality;
    public bool IsUnique => _isUnique;
    public EffectType EffectType => _effectType;
    public string EffectDescription => _effectDescription;
    
    public virtual void Modify(GlobalEffect effect) {}
    
    public void SetItemTypeNames(List<string> itemTypeNames)
    {
        _instanceItemTypeContainers.ForEach(itemTypeContainer => itemTypeContainer.AllEffectTypeNames = itemTypeNames);
    }
}

public enum EffectType
{
    Positive,
    Negative,
    Netral
}