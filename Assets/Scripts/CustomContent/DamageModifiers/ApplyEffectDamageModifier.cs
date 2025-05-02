using Combat;

public abstract class ApplyEffectDamageModifier : DamageModifier
{
    public override float Modify(CombatEntity otherEntity, float value)
    {
        ApplyEffect(otherEntity.ComponentsContainer.Get<EntityEffectManager>());
        
        return value;
    }

    protected abstract void ApplyEffect(EntityEffectManager effectManager);
}