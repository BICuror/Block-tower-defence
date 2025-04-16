using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using System;

public abstract class GlobalEffectData : ScriptableObject
{
    [Header("GlobalEffectData")]
    [Range(-5, 5)] [SerializeField] private int _quality = 3;
    [SerializeField] private bool _isUnique = false;
    [Dropdown("AllEffectTypeNames")] [SerializeField] private string _effectTypeName;
    
    [Header("EffectAperanceCondition")]
    [SerializeField] private EffectApperanceConditionData _effectApperanceCondition;
    [SerializeField] private ArgumentsContainer _argumentsContainer;
    
    [HideInInspector] public List<string> AllEffectTypeNames;

    [Header("TooltipData")] 
    [SerializeField] private string _effectName;
    [SerializeField] private string _effectDescription;
    
    public ArgumentsContainer ArgumentsContainer => _argumentsContainer;
    public EffectApperanceConditionData EffectApperanceCondition => _effectApperanceCondition;
    public Type EffectType => Type.GetType(_effectTypeName);
    public bool HasApperanceCondition => _effectApperanceCondition;
    public int Quality => _quality;
    public bool IsUnique => _isUnique;
    public string EffectName => _effectName;
    public string EffectDescription => _effectDescription;
    
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(_effectTypeName) || EffectType == null)
        {
            Debug.LogError("Invalid modifier type " + _effectTypeName);
        }
    }
}