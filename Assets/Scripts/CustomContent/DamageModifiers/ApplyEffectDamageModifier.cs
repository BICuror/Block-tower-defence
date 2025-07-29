using System;
using Combat;

public sealed class ApplyEffectDamageModifier : DamageModifier
{
    private int _appliedStrength;
    private Type _entityEffectType;
    private float _effectDuration;
    
    public override void Initialize()
    {
        _appliedStrength = Args.GetArgument<int>("EffectStrength");
        _effectDuration = Args.GetArgument<float>("EffectDuration");
        _entityEffectType = Type.GetType(Args.GetArgument<string>("EffectTypeName"));
    }
    
    public override float Modify(CombatEntity otherEntity, float value)
    {
        otherEntity.ComponentsContainer.Get<EntityEffectManager>().TryApplyTemporaryEffect(_entityEffectType, _appliedStrength, _effectDuration);
        
        return value;
    }
}