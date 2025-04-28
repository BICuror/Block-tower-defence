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
    [SerializeField] private string _modificatorName;
    [SerializeField] private string _modificatorDescription;
    
    [HideInInspector] public List<string> AllEffectTypeNames;
    
    public ArgumentsContainer ArgumentsContainer => _argumentsContainer;
    public virtual Type EffectType => Type.GetType(_effectTypeName);
    public string ModificatorName => _modificatorName;
    public string ModificatorDescription => _modificatorDescription;

    public virtual void Modify(EntityModificator modificator) {}
}