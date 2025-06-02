using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class GlobalEffectData : ScriptableObject
{
    [Header("GlobalEffectData")]
    [Range(1, 5)] [SerializeField] private int _quality = 3;
    [SerializeField] private EffectType _effectType;
    [SerializeField] private bool _isUnique;
    [SerializeField] private List<InstanceItemTypeContainer> _instanceItemTypeContainers;
    [SerializeField] private ArgumentsContainer _argumentsContainer;

    [Space] [Header("EffectAppearanceCondition")] 
    [SerializeField] private bool _hasEffectAppearanceCondition;
    [AllowNesting] [ShowIf("_hasEffectAppearanceCondition")] [SerializeField] private EffectAppearanceConditionData _effectAppearanceCondition;

    [Space] [Header("TooltipData")] 
    [SerializeField] private string _effectName;
    [TextArea] [SerializeField] private string _effectDescription;
    
    public List<InstanceItemTypeContainer> InstanceItemTypeContainers => _instanceItemTypeContainers;
    public ArgumentsContainer ArgumentsContainer => _argumentsContainer;
    public EffectAppearanceConditionData EffectAppearanceCondition => _effectAppearanceCondition;
    public bool HasAppearanceCondition => _hasEffectAppearanceCondition;
    public int Quality => _quality;
    public bool IsUnique => _isUnique;
    public EffectType EffectType => _effectType;
    public string EffectName => _effectName;
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