using Combat;

public sealed class AddChargeOnHit : DamageModifier
{
    private ChargeDuration _chargeDuration;
    private float _chargePerAttack;
    
    public override void Initialize()
    {
        _chargeDuration = OwnerEntity.StatContainer.Get<ChargeDuration>();
        _chargePerAttack = Args.GetArgument<float>("ChargePerAttack");
    }
    
    public override float Modify(CombatEntity otherEntity, float value)
    {
        OwnerEntity.ComponentsContainer.Get<InfernoTower>().AddCharge(_chargeDuration.Value * _chargePerAttack);

        return value;
    }
}