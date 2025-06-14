using Combat;

public sealed class AddAdditionalStackToEveryEffect : DamageModifier
{
    public override float Modify(CombatEntity otherEntity, float value)
    {
        int strength = Args.GetArgument<int>("AdditionalStackCount");
        float duration = Args.GetArgument<float>("EffectDuration");
        
        EntityEffectManager effectManager = otherEntity.ComponentsContainer.Get<EntityEffectManager>();
        
        effectManager.AppliedEffectTypes.ForEach(appliedEffectType =>
        {
            effectManager.TryApplyTemporaryEffect(appliedEffectType, strength, duration);
        });

        return value;
    }
}