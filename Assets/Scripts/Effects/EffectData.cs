using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using System;

public abstract class EffectData : ScriptableObject
{
    [Header("EffectData")]
    [Range(-5, 5)] [SerializeField] private int _quality = 3;
    [SerializeField] private bool _isUnique = false;
    [Dropdown("AllEffectTypeNames")] [SerializeField] private string _effectTypeName;
    
    [Header("EffectAperanceCondition")]
    [SerializeField] private EffectApperanceConditionData _effectApperanceCondition;
    [SerializeField] private ArgumentsContainer _argumentsContainer;
    
    [HideInInspector] public List<string> AllEffectTypeNames;
    
    public ArgumentsContainer ArgumentsContainer => _argumentsContainer;
    public EffectApperanceConditionData EffectApperanceCondition => _effectApperanceCondition;
    public Type EffectType => Type.GetType(_effectTypeName);
    public bool HasApperanceCondition => _effectApperanceCondition;
    public int Quality => _quality;
    public bool IsUnique => _isUnique;
    
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(_effectTypeName) || EffectType == null)
        {
            Debug.LogError("Invalid modifier type " + _effectTypeName);
        }
    }
}