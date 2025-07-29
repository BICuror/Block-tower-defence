using Combat;

public sealed class DealMoreToLowHpDamageModifier : DamageModifier
{
    private float _damageMultiplier;
    private float _threshold;
    
    public override void Initialize()
    {
        _damageMultiplier = Args.GetArgument<float>("Multiplier");
        _threshold = Args.GetArgument<float>("Threshold");
    }
    
    public override float Modify(CombatEntity otherEntity, float value)
    {
        if (otherEntity.Health.GetHpPercent() <= _threshold)
        {
            return value * _damageMultiplier;
        }
        
        return value;
    }
}