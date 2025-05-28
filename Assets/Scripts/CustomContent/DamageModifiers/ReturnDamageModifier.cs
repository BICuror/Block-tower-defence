using Combat;

public class ReturnDamageModifier : DamageModifier
{
    public override float Modify(CombatEntity otherEntity, float value)
    {
        if (otherEntity.Health.IsAlive())
        {
            otherEntity.Health.ReceiveEnemyDamage(value, otherEntity);
        }
        
        return value;
    }
}