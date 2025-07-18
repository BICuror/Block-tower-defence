using System.Collections.Generic;
using System.Linq;
using Combat;
using System;

public sealed class DamageModifierContainer
{
    private ListDictionary<Type, DamageModifier> _modifiersDictionaryList;
    private DamageModifier _currentSingleDamageModifier;
    private CombatEntity _ownerEntity;

    public DamageModifierContainer(CombatEntity ownerEntity)
    {
        _modifiersDictionaryList = new(SortDamageModifiers);
        _ownerEntity = ownerEntity;
    }
    
    public void Add(Type modifierType) 
    {
        DamageModifier modifier = (DamageModifier)Activator.CreateInstance(modifierType);

        Add(modifier);
    }

    public void Add(DamageModifier modifier)
    {
        modifier.SetOwner(_ownerEntity);
                
        if (modifier.Order != ResolveOrder.Single)
        {
            _modifiersDictionaryList.Add(modifier.GetType(), modifier);
        }
        else
        {
            _currentSingleDamageModifier = modifier;
        }
    }
    
    public void Remove(Type modifierType)
    {
        if (_modifiersDictionaryList.Contains(modifierType))
        {
            _modifiersDictionaryList.Remove(modifierType);
        }
        else if (_currentSingleDamageModifier != null)
        {
            _currentSingleDamageModifier = null;
        }
    }

    public void Remove(DamageModifier modifier)
    {
        Type modifierType = modifier.GetType();
        
        if (_currentSingleDamageModifier == modifier)
        {
            _currentSingleDamageModifier = null;
        }
        else if (_modifiersDictionaryList.Contains(modifierType))
        {
            _modifiersDictionaryList.Remove(modifierType, modifier);
        }
    }
    
    public float Modify(float value, CombatEntity otherEntity)
    {
        value = ApplyModifiers(otherEntity, value);

        return value;
    }

    private float ApplyModifiers(CombatEntity otherEntity, float value)
    {
        if (_currentSingleDamageModifier != null) return _currentSingleDamageModifier.Modify(otherEntity, value);

        List<DamageModifier> damageModifiers = _modifiersDictionaryList.GetAllItems();
        
        for (int i = 0; i < damageModifiers.Count; i++) 
        { 
            value = damageModifiers[i].Modify(otherEntity, value);
        }
            
        return value;
    }
    
    private List<DamageModifier> SortDamageModifiers(List<DamageModifier> damageModifiers) => damageModifiers.OrderBy(mod => mod.Order).ToList();
    
    #region Generic
    public void Add<T>()
    {
        Type t = typeof(T);
        Add(t);
    }
    
    public void Remove<T>()
    {
        Type t = typeof(T);
        Remove(t);
    }
    #endregion
}