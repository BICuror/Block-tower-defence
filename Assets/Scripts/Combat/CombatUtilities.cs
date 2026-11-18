namespace Combat
{
    public static class CombatUtilities
    {
        public const float MINIMAL_NONLETHAL_HEALTH = 1f;
    
        public static void TakeMaxNonLethalDamage(CombatEntity entity)
        { 
            entity.Health.ReceiveEffectDamage(entity.Health.GetHp() - MINIMAL_NONLETHAL_HEALTH, DamageVisualsType.NonLethal);
        }
        
        public static bool TryTakeNonLethalDamage(CombatEntity entity, float damage)
        {
            bool result = CanTakeNonLethalDamage(entity, damage);
            
            if (result)
            {
                entity.Health.ReceiveEffectDamage(damage, DamageVisualsType.NonLethal);
            }
            
            return result;
        }
        
        public static bool CanTakeNonLethalDamage(CombatEntity entity, float damage)
        {
            return entity.Health.GetHp() > damage + MINIMAL_NONLETHAL_HEALTH;
        }
    
        public static void ExecuteEntity(CombatEntity entity)
        {
            entity.Health.Die();
        }
    }
}