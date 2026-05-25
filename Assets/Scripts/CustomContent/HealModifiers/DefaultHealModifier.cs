using Combat;

public sealed class DefaultHealModifier : DamageModifier
{
    private float _flatHealModifier;
    private float _healMultiplier;
    
    public override void Initialize()
    {
        _flatHealModifier = Args.GetArgument<float>("FlatHealModifier");
        _healMultiplier = Args.GetArgument<float>("HealMultiplier");
    }
    
    public override float Modify(CombatEntity otherEntity, float value)
    {
        return (value + _flatHealModifier) * _healMultiplier;
    }
}