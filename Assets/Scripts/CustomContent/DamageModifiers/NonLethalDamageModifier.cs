using Combat;

public class NonLethalDamageModifier : DamageModifier
{
    public virtual ResolveOrder Order => ResolveOrder.Final;
    
    public override float Modify(CombatEntity otherEntity, float value)
    {
        if (otherEntity.Health.GetHp() > value) otherEntity.Health.ReceiveEffectDamage(value);
        else otherEntity.Health.ReceiveEffectDamage(otherEntity.Health.GetHp() - 0.01f);
        
        return 0;
    }
}