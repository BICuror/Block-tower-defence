using Combat;

public sealed class DealMoreToLowHpDamageModifier : DamageModifier
{
    public override float Modify(CombatEntity otherEntity, float value)
    {
        if (otherEntity.Health.GetHpPercent() <= 0.5)
        {
            return value * 2f;
        }
        
        return value;
    }
}