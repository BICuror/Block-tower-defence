using Combat;

public sealed class ExecuteEntitiesInArea : EntityModificator
{
    private float _executeThreshold;
    
    public override void Enable()
    {
        _executeThreshold = Args.GetArgument<float>("Threshold");
        
        AreaEntityDetector areaEntityDetector = Entity.ComponentsContainer.Get<AreaEntityDetector>();
        
        areaEntityDetector.AddedItem += SubscribeToEntity;
        areaEntityDetector.RemovedItem += UnsubscribeFromEntity;
        
        foreach (CombatEntity combatEntity in areaEntityDetector.GetList())
        {
            SubscribeToEntity(combatEntity);
        }
    }

    private void SubscribeToEntity(CombatEntity combatEntity)
    {
        combatEntity.Health.EntityDamaged += TryExecuteEntity;
        combatEntity.Health.EntityDied += UnsubscribeFromEntity;
        
        TryExecuteEntity(combatEntity);
    }

    private void UnsubscribeFromEntity(CombatEntity combatEntity)
    {
        combatEntity.Health.EntityDamaged -= TryExecuteEntity;
        combatEntity.Health.EntityDied -= UnsubscribeFromEntity;
    }

    private void TryExecuteEntity(CombatEntity combatEntity)
    {
        if (combatEntity.Health.GetHpPercent() <= _executeThreshold)
        {
            combatEntity.Health.ReceiveEnemyDamage(combatEntity.StatContainer.Get<MaxHealth>().Value, Entity);
        }
    }

    public override void Disable()
    {
        AreaEntityDetector areaEntityDetector = Entity.ComponentsContainer.Get<AreaEntityDetector>();
        areaEntityDetector.AddedItem -= TryExecuteEntity;
    }
}
