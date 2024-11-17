using System.Collections.Generic;
using UnityEngine;
using Combat;

public sealed class DamageModifierContainer : MonoBehaviour
{
    private List<DamageModifier> _initialModifiersList = new();
    private List<DamageModifier> _defaultModifiersList = new();
    private List<DamageModifier> _finalModifiersList = new();

    public void Add(DamageModifier modifier)
    {
        switch (modifier.Order)
        {
            case ResolveOrder.Default: _initialModifiersList.Add(modifier); break;
            case ResolveOrder.Initial: _initialModifiersList.Add(modifier); break;
            case ResolveOrder.Final: _finalModifiersList.Add(modifier); break;
        }
    }
    
    public void Remove(DamageModifier modifier)
    {
        switch (modifier.Order)
        {
            case ResolveOrder.Default: _initialModifiersList.Remove(modifier); break;
            case ResolveOrder.Initial: _initialModifiersList.Remove(modifier); break;
            case ResolveOrder.Final: _finalModifiersList.Remove(modifier); break;
        }
    }
    
    public float Modify(CombatEntity otherEntity, float value)
    {
        value = ApplyModifiers(_initialModifiersList, otherEntity, value);
        value = ApplyModifiers(_defaultModifiersList, otherEntity, value);
        value = ApplyModifiers(_finalModifiersList, otherEntity, value);

        return value;
    }

    private float ApplyModifiers(List<DamageModifier> damageModifiers, CombatEntity otherEntity, float value)
    {
        damageModifiers.ForEach(modifier =>
        {
            if (otherEntity.Health.IsAlive())
            {
                value = modifier.Modify(otherEntity, value);
            }
        });

        return value;
    }
}
