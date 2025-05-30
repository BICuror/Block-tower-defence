using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using System;

public abstract class GlobalEffectData : ScriptableObject
{
    [Header("GlobalEffectData")]
    [Range(1, 5)] [SerializeField] private int _quality = 3;
    [SerializeField] private EffectType _effectType;
    
    [SerializeField] private bool _isUnique = false;
    [Dropdown("AllEffectTypeNames")] [SerializeField] private string _effectTypeName;
    
    [Header("EffectAppearanceCondition")]
    [SerializeField] private EffectAppearanceConditionData effectAppearanceCondition;
    [ShowIf("HasAppearanceCondition", true)] [SerializeField] private ArgumentsContainer _argumentsContainer;
    
    [HideInInspector] public List<string> AllEffectTypeNames;

    [Header("TooltipData")] 
    [TextArea] [SerializeField] private string _effectDescription;
    
    public virtual Type EffectInstanceType => Type.GetType(_effectTypeName);
    public ArgumentsContainer ArgumentsContainer => _argumentsContainer;
    public EffectAppearanceConditionData EffectAppearanceCondition => effectAppearanceCondition;
    public bool HasAppearanceCondition => effectAppearanceCondition;
    public int Quality => _quality;
    public bool IsUnique => _isUnique;
    public EffectType EffectType => _effectType;
    public string EffectDescription => _effectDescription;
}

public enum EffectType
{
    Positive,
    Negative,
    Netral
}