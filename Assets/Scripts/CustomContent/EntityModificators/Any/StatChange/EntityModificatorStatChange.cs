using System.Collections.Generic;
using System;

public sealed class EntityModificatorStatChange : EntityModificator
{
    private Dictionary<Type, StatModifier> _modifiers = new();

    public override bool CanBeApplied()
    {
        foreach (StatChange statChange in ModificatorConfig.StatChanges)
        {
            Type statType = statChange._statConfig.GetStatType();
            
            if (Entity.StatContainer.Has(statType))
            {
                return true;
            }
        }

        return false;
    }
    
    public override void Enable()
    {
        ModificatorConfig.StatChanges.ForEach(statChange =>
        {
            Type statType = statChange._statConfig.GetStatType();
            
            if (Entity.StatContainer.Has(statType))
            {
                StatModifier modifier = new StatModifier();
                
                _modifiers.Add(statType, modifier);
                
                modifier.SetFlat(statChange.FlatChange);
                modifier.SetMultiplier(statChange.MultiplierChange);
                
                Entity.StatContainer.Get(statType).AddStatModifier(modifier);
            }
        });
    }

    public override void Disable()
    {
        foreach (Type statType in _modifiers.Keys)
        {
            Entity.StatContainer.Get(statType).RemoveStatModifier(_modifiers[statType]);
        }
        
        _modifiers.Clear();
    }
}