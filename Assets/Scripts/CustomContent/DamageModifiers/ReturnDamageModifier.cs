using System.Collections.Generic;
using Combat;

public class ReturnDamageModifier : DamageModifier
{
    public override float Modify(CombatEntity otherEntity, float value)
    {
        IReadOnlyList<CombatEntity> entitiesInArea;
        
        if (OwnerEntity.ComponentsContainer.Has<EnemyAreaScaner>() && Args.GetArgument<bool>("IsBuildingModificator"))
        {
            entitiesInArea = OwnerEntity.ComponentsContainer.Get<EnemyAreaScaner>().GetList();
        }
        else
        {
            entitiesInArea = OwnerEntity.ComponentsContainer.Get<BuildingAreaScaner>().GetList();
        }

        for (int i = 0; i < entitiesInArea.Count; i++)
        {
            entitiesInArea[i].Health.ReceiveEnemyDamage(value, OwnerEntity);
        }
        
        return value;
    }
}