using System.Collections.Generic;
using Combat;

public sealed class AttackAllEnemiesInAreaEntityModificator : EntityModificator
{
    public override void Enable() => Entity.Health.Damaged += AttackAllEnemiesInArea;

    public override void Disable() => Entity.Health.Damaged -= AttackAllEnemiesInArea;
    
    private void AttackAllEnemiesInArea()
    {
        IReadOnlyList<CombatEntity> entitiesInArea = Entity.ComponentsContainer.Get<AreaEntityDetector>().GetList();

        for (int i = 0; i < entitiesInArea.Count; i++)
        {
            entitiesInArea[i].Health.ReceiveEnemyDamage(Entity.StatContainer.Get<Damage>().Value, Entity);
        }
    }
}