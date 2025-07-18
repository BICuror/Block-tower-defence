using Combat;

public sealed class DealMoreToFullHpDamageModifier : DamageModifier
{
    public override float Modify(CombatEntity otherEntity, float value)
    {
        if (otherEntity.Health.GetHpPercent() == 1f)
        {
            return value * Args.GetArgument<float>("DamageMultiplier");
        }
        
        return value;
    }
}