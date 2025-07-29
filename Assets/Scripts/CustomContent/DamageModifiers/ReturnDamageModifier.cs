using System.Collections.Generic;
using Combat;

public class ReturnDamageModifier : DamageModifier
{
    public override float Modify(CombatEntity otherEntity, float value)
    {
        IReadOnlyList<CombatEntity> entitiesInArea = OwnerEntity.ComponentsContainer.Get<AreaEntityDetector>().GetList();

        for (int i = 0; i < entitiesInArea.Count; i++)
        {
            entitiesInArea[i].Health.ReceiveEnemyDamage(OwnerEntity.StatContainer.Get<Damage>().Value, OwnerEntity);
        }
        
        return value;
    }
}