using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "EffectApperanceCondition", menuName = "Effects/EffectApperanceCondition")]

public class EffectApperanceConditionData : ScriptableObject
{
    [Header("EffectApperanceCondition")]
    [Dropdown("AllConditioinTypeNames")] [SerializeField] private string _apperanceConditionTypeName;
    [SerializeField] private ArgumentsContainer _argumentsContainer;
    
    [HideInInspector] public List<string> AllConditioinTypeNames;
    
    public Type ApperanceConditionType => Type.GetType(_apperanceConditionTypeName);
    public ArgumentsContainer ArgumentsContainer => _argumentsContainer;
    
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(_apperanceConditionTypeName) || AllConditioinTypeNames == null)
        {
            Debug.LogError($"Invalid apperance condition type in {name}");
        }
    }
}