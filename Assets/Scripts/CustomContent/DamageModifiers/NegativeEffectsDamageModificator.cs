using Combat;

public sealed class NegativeEffectsDamageModificator : DamageModifier
{
    private float _damageMultiplier;
    private bool _isDamageDealerModifier;
    
    public override void Initialize()
    {
        _isDamageDealerModifier = Args.GetArgument<bool>("IsDealerDamageModifier");
        _damageMultiplier = Args.GetArgument<float>("DamageModifierValueDecrease");
    }
    
    public override float Modify(CombatEntity otherEntity, float value)
    {
        if (_isDamageDealerModifier)
        {
            if (otherEntity.ComponentsContainer.Get<EntityEffectManager>().HasEffect(EntityEffectType.Negative))
            {
                value *= 1 - _damageMultiplier;
            }
        }
        else
        {
            if (OwnerEntity.ComponentsContainer.Get<EntityEffectManager>().HasEffect(EntityEffectType.Negative))
            {
                value *= 1 - _damageMultiplier;
            }
        }
        
        
        return value;
    }
}