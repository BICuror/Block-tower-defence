using Combat;

public sealed class HealDamageModifier : DamageModifier
{
    public override float Modify(CombatEntity otherEntity, float value)
    {
        OwnerEntity.Health.ReceivePercentHeal(Args.GetArgument<float>("HealPercent"));
        
        return value;
    }
}