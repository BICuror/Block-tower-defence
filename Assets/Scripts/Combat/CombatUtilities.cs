using UnityEngine;
using Combat;

public static class CombatUtilities
{
    private const float ISLOATION_CUBE_RADIUS = 1f;

    public static bool EntityIsIsolated(GameObject gameObject)
    {
        return Physics.OverlapSphere(gameObject.transform.position, ISLOATION_CUBE_RADIUS, gameObject.layer).Length <= 1;
    }

    public static bool TryTakeNonLethalDamage(CombatEntity entity, float damage)
    {
        bool result = CanTakeNonLethalDamage(entity, damage);
        
        if (result)
        {
            entity.Health.ReceiveEffectDamage(damage);
        }
        
        return result;
    }
    
    public static bool CanTakeNonLethalDamage(CombatEntity entity, float damage)
    {
        return entity.Health.GetHp() > damage;
    }

    public static void ExecuteEntity(CombatEntity entity)
    {
        entity.Health.Die();
    }
}