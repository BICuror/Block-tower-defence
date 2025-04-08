using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "EntityModificatorData", menuName = "EntityModificatorData")]

public class EntityModificatorData : ScriptableObject
{
    [Header("EffectData")]
    [Dropdown("AllEffectTypeNames")] [SerializeField] private string _effectTypeName;
    [SerializeField] private ArgumentsContainer _argumentsContainer;
 
    [Header("UI Data")]
    [SerializeField] private string _modificatorName;
    [SerializeField] private string _modificatorDescription;
    
    [HideInInspector] public List<string> AllEffectTypeNames;
    
    public ArgumentsContainer ArgumentsContainer => _argumentsContainer;
    public Type EffectType => Type.GetType(_effectTypeName);
    public string ModificatorName => _modificatorName;
    public string ModificatorDescription => _modificatorDescription;
    
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(_effectTypeName) || EffectType == null)
        {
            Debug.LogError("Invalid modifier type " + _effectTypeName);
        }
    }
}