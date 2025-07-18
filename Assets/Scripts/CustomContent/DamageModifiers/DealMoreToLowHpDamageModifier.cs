using Combat;

public sealed class DealMoreToLowHpDamageModifier : DamageModifier
{
    public override float Modify(CombatEntity otherEntity, float value)
    {
        if (otherEntity.Health.GetHpPercent() <= Args.GetArgument<float>("Threshold"))
        {
            return value * Args.GetArgument<float>("Multiplier");
        }
        
        return value;
    }
}