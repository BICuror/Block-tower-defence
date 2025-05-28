using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "EntityModificatorData", menuName = "EntityModificatorData")]

public class EntityModificatorData : ScriptableObject
{
    [Header("GlobalEffectData")]
    [Dropdown("AllEffectTypeNames")] [SerializeField] private string _effectTypeName;
    [SerializeField] private ArgumentsContainer _argumentsContainer;
 
    [Header("UI Data")]
    [SerializeField] private EffectType _effectType;
    [SerializeField] private string _modificatorName;
    [SerializeField] private string _modificatorDescription;
    
    [HideInInspector] public List<string> AllEffectTypeNames;
    
    public ArgumentsContainer ArgumentsContainer => _argumentsContainer;
    public virtual Type ModificatorInstanceType => Type.GetType(_effectTypeName);
    public EffectType EffectType => _effectType;
    public string ModificatorName => _modificatorName;
    public string ModificatorDescription => _modificatorDescription;

    public virtual void Modify(EntityModificator modificator) {}
}