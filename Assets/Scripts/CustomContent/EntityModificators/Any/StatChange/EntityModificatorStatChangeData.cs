using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "EntityModificatorData", menuName = "EntityModificators/EntityStatChangeModificatorData")]

public sealed class EntityModificatorStatChangeData : EntityModificatorData
{
    [SerializeField] private List<StatChange> _statChanges;
    
    public override Type ModificatorInstanceType => typeof(EntityModificatorStatChange);
    
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
    public StatData StatData;
    public float FlatChange;
    public float MultiplierChange;
}