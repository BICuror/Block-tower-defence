using Combat;

public sealed class TakeLessDamageWhenAffectedByNegativeEntityEffects : DamageModifier
{
    public override float Modify(CombatEntity otherEntity, float value)
    {
        if (OwnerEntity.ComponentsContainer.Get<EntityEffectManager>().HasEffect(EntityEffectType.Negative))
        {
            value *= Args.GetArgument<float>("DamageModifier");
        }
        
        return value;
    }
}