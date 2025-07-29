using Combat;

public sealed class DealMoreToFullHpDamageModifier : DamageModifier
{
    private float _damageMultiplier;
    
    public override void Initialize()
    {
        _damageMultiplier = Args.GetArgument<float>("DamageMultiplier");
    }
    
    public override float Modify(CombatEntity otherEntity, float value)
    {
        if (otherEntity.Health.GetHpPercent() == 1f)
        {
            return value * _damageMultiplier;
        }
        
        return value;
    }
}