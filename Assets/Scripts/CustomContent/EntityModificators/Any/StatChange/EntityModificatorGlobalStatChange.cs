using System.Collections.Generic;
using Zenject;
using System;

public class EntityModificatorGlobalStatChange : EntityModificator
{
    [Inject] private GlobalStatContainer _globalStatContainer;
    
    private Dictionary<Type, StatModifier> _modifiers = new();
    
    public override void Enable()
    {
        ModificatorData.StatChanges.ForEach(statChange =>
        {
            Type statType = statChange.StatData.GetStatType();
            
            StatModifier modifier = new StatModifier();
            
            _modifiers.Add(statType, modifier); 
            _globalStatContainer.Get(statType).AddStatModifier(modifier);
            
            modifier.SetFlat(statChange.FlatChange); 
            modifier.SetMultiplier(statChange.MultiplierChange);
        });
    }

    public override void Disable()
    {
        foreach (Type statType in _modifiers.Keys)
        {
            _globalStatContainer.Get(statType).RemoveStatModifier(_modifiers[statType]);
        }
    }
}