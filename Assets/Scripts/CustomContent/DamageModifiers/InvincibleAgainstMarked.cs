using Combat;

public class InvincibleAgainstMarked : DamageModifier
{
    public override ResolveOrder Order => ResolveOrder.Final;
    
    public override float Modify(CombatEntity otherEntity, float value)
    {
        if (otherEntity.ComponentsContainer.Get<EntityEffectManager>().HasEffect(typeof(MarkEffect)))
        {
            return 0f;
        }

        return value;
    }
}
