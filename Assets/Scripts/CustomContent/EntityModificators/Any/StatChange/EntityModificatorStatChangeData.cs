using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "EntityModificatorData", menuName = "EntityStatChangeModificatorData")]

public sealed class EntityModificatorStatChangeData : EntityModificatorData
{
    [SerializeField] private List<StatChange> _statChanges;
    
    public override Type EffectType => typeof(EntityModificatorStatChange);
    
    public override void Modify(EntityModificator modificator)
    {
        EntityModificatorStatChange statChange = (EntityModificatorStatChange)modificator;
        
        statChange.SetStatChanges(_statChanges);
    }

    private void OnValidate()
    {
        AllEffectTypeNames = new List<string>{"EntityModificatorStatChange"};
    }
}
 
[Serializable] public sealed class StatChange
{
    public string StatTypeName;
    public float FlatChange;
    public float MultiplierChange;
}