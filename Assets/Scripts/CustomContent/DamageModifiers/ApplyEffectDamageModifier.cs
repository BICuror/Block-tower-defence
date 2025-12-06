using System;
using Combat;

public sealed class ApplyEffectDamageModifier : DamageModifier
{
    private int _appliedStrength;
    private Type _entityEffectType;
    private float _effectDuration;
    private bool _hasEffectDuration;
    
    public override void Initialize()
    {
        _appliedStrength = Args.GetArgument<int>("EffectStrength");
        _entityEffectType = Type.GetType(Args.GetArgument<string>("EffectTypeName"));

        _hasEffectDuration = Args.HasArgument("EffectDuration");
        
        if (_hasEffectDuration) _effectDuration = Args.GetArgument<float>("EffectDuration");
    }
    
    public override float Modify(CombatEntity otherEntity, float value)
    {
        if (_hasEffectDuration)
        {
            otherEntity.ComponentsContainer.Get<EntityEffectManager>().TryApplyTemporaryEffect(_entityEffectType, _appliedStrength, _effectDuration);
        }
        else
        {
            otherEntity.ComponentsContainer.Get<EntityEffectManager>().TryApplyEffect(_entityEffectType, _appliedStrength);
        }
        
        return value;
    }
}