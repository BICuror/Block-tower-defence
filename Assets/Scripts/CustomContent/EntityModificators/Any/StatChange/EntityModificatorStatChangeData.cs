using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "EntityModificatorData", menuName = "EntityModificators/EntityStatChangeModificatorData")]

public sealed class EntityModificatorStatChangeData : EntityModificatorData
{
    [SerializeField] private List<StatChange> _statChanges;
    
    public override void Modify(EntityModificator modificator)
    {
        if (modificator is not EntityModificatorStatChange) return;
        
        EntityModificatorStatChange statChange = (EntityModificatorStatChange)modificator;
        
        statChange.SetStatChanges(_statChanges);
    }
}
 
[Serializable] public sealed class StatChange
{
    public StatData StatData;
    public float FlatChange;
    public float MultiplierChange;
}