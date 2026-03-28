using Combat;

public class NonLethalDamageModifier : DamageModifier
{
    public override ResolveOrder Order => ResolveOrder.Final;
    
    public override float Modify(CombatEntity otherEntity, float value)
    {
        if (otherEntity.Health.GetHp() <= value + CombatUtilities.MINIMAL_NONLETHAL_HEALTH)
        {
            value = otherEntity.Health.GetHp() - CombatUtilities.MINIMAL_NONLETHAL_HEALTH;
        }
        
        return value;
    }
}