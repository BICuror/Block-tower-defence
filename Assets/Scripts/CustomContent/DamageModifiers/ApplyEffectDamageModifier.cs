using System;
using Combat;

public sealed class ApplyEffectDamageModifier : DamageModifier
{
    private int _appliedStrength;
    private Type _entityEffectType;
    private float _effectDuration;

    public void SetEffectData(Type entityEffectType, int appliedStrength, float effectDuration)
    {
        _entityEffectType = entityEffectType;
        _appliedStrength = appliedStrength;
        _effectDuration = effectDuration;
    }
    
    public override float Modify(CombatEntity otherEntity, float value)
    {
        otherEntity.ComponentsContainer.Get<EntityEffectManager>().TryApplyTemporaryEffect(_entityEffectType, _appliedStrength, _effectDuration);
        
        return value;
    }
}