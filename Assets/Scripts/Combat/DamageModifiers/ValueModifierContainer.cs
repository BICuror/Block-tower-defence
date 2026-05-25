using System.Collections.Generic;
using System.Linq;
using Combat;
using System;

public sealed class ValueModifierContainer
{
    private ListDictionary<Type, DamageModifier> _modifiersDictionaryList;
    private CombatEntity _ownerEntity;

    public ValueModifierContainer(CombatEntity ownerEntity)
    {
        _modifiersDictionaryList = new(SortDamageModifiers);
        _ownerEntity = ownerEntity;
    }
    
    public void Add(DamageModifier modifier)
    {
        modifier.SetOwner(_ownerEntity);
        modifier.Initialize();
        
        _modifiersDictionaryList.Add(modifier.GetType(), modifier);
    }

    public void Remove(DamageModifier modifier)
    {
        Type modifierType = modifier.GetType();
        
        _modifiersDictionaryList.Remove(modifierType, modifier);
    }
    
    public float ModifyByAllModificators(float value, CombatEntity otherEntity)
    {
        List<DamageModifier> damageModifiers = _modifiersDictionaryList.GetAllItems();
        
        for (int i = 0; i < damageModifiers.Count; i++) 
        { 
            value = damageModifiers[i].Modify(otherEntity, value);
        }
            
        return value;
    }
    
    private List<DamageModifier> SortDamageModifiers(List<DamageModifier> damageModifiers) => damageModifiers.OrderBy(mod => mod.Order).ToList();
}