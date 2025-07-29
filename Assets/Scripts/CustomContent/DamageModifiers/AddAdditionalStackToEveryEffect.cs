using Combat;

public sealed class AddAdditionalStackToEveryEffect : DamageModifier
{
    private int _additionalStackAmount;
    private float _duration;
    
    public override void Initialize()
    {
        _additionalStackAmount = Args.GetArgument<int>("AdditionalStackCount");
        _duration = Args.GetArgument<float>("EffectDuration");
    }
    
    public override float Modify(CombatEntity otherEntity, float value)
    {
        EntityEffectManager effectManager = otherEntity.ComponentsContainer.Get<EntityEffectManager>();
        
        effectManager.AppliedEffectTypes.ForEach(appliedEffectType =>
        {
            if (effectManager.AppliedEffects[appliedEffectType].EffectType == EntityEffectType.Negative)
            {
                effectManager.TryApplyTemporaryEffect(appliedEffectType, _additionalStackAmount, _duration);
            }
        });

        return value;
    }
}