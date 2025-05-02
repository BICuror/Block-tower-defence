using System.Collections.Generic;
using System;

public sealed class EntityModificatorStatChange : EntityModificator
{
    private Dictionary<Type, StatModifier> _modifiers = new();
    private List<StatChange> _statChanges;

    public void SetStatChanges(List<StatChange> statChanges)
    {
        _statChanges = statChanges;
    }

    public override bool CanBeApplied()
    {
        foreach (StatChange statChange in _statChanges)
        {
            Type statType = Type.GetType(statChange.StatTypeName);
            
            if (Entity.StatContainer.Has(statType))
            {
                return true;
            }
        }

        return false;
    }
    
    public override void Enable()
    {
        _statChanges.ForEach(statChange =>
        {
            Type statType = Type.GetType(statChange.StatTypeName);
            
            if (Entity.StatContainer.Has(statType))
            {
                StatModifier modifier = new StatModifier();
                
                _modifiers.Add(statType, modifier);
                Entity.StatContainer.Get(statType).AddStatModifier(modifier);
                
                modifier.SetFlat(statChange.FlatChange);
                modifier.SetMultiplier(statChange.MultiplierChange);
            }
        });
    }

    public override void Disable()
    {
        foreach (Type statType in _modifiers.Keys)
        {
            Entity.StatContainer.Get(statType).RemoveStatModifier(_modifiers[statType]);
        }
    }
}