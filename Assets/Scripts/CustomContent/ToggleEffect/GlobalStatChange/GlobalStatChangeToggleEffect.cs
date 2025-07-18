using System.Collections.Generic;
using Zenject;
using System;

public sealed class GlobalStatChangeToggleEffect : GlobalToggleEffect
{
    [Inject] private GlobalStatContainer _globalStatContainer;
    
    private Dictionary<Type, StatModifier> _modifiers = new();
    private List<StatChange> _statChanges;

    public void SetStatChanges(List<StatChange> statChanges)
    {
        _statChanges = statChanges;
    }
    
    public override void Enable()
    {
        _statChanges.ForEach(statChange =>
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