using Combat;

public sealed class HealDamageModifier : DamageModifier
{
    public override float Modify(CombatEntity otherEntity, float value)
    {
        OwnerEntity.Health.ReceivePercentHeal(0.05f);
        
        return value;
    }
}