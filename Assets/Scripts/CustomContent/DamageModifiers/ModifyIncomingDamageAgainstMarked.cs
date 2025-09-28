using Combat;

public sealed class ModifyIncomingDamageAgainstMarked : DamageModifier
{
    public override ResolveOrder Order => ResolveOrder.Final;
    
    public override float Modify(CombatEntity otherEntity, float value)
    {
        if (otherEntity.ComponentsContainer.Get<EntityEffectManager>().HasEffect(typeof(MarkEffect)))
        {
            return value * Args.GetArgument<float>("DamageMultiplier");
        }

        return value;
    }
}
