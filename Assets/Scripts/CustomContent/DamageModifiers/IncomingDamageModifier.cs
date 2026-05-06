using Combat;

public sealed class IncomingDamageModifier : DamageModifier
{
    private float _damageModifier;

    public override void Initialize()
    {
        _damageModifier = Args.GetArgument<float>("DamageModifier");
    }
    
    public override float Modify(CombatEntity otherEntity, float value) => value * _damageModifier;
}