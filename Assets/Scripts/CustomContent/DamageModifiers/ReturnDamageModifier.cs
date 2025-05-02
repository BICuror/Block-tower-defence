using Combat;

public class ReturnDamageModifier : DamageModifier
{
    public override float Modify(CombatEntity otherEntity, float value)
    {
        if (otherEntity.Health.IsAlive())
        {
            float backDamage = OwnerEntity.DamageModifierContainer.DealerContainer.Modify(value, otherEntity);
            
            otherEntity.Health.ReceiveEnemyDamage(backDamage, otherEntity);
        }
        
        return value;
    }
}