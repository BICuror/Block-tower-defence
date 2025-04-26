using Combat;

public sealed class InvincibilityDamageModifier : DamageModifier
{
    public override ResolveOrder Order => ResolveOrder.Single;

    public override float Modify(CombatEntity otherEntity, float value)
    {
        return 0;
    }
}
