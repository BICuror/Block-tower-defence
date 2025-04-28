using Zenject;
using Combat;

public sealed class GlobalEnemyModificatorToggleEffect : GlobalToggleEffect
{
    [Inject] private GlobalEnemyContainer _globalEnemyContainer;
    private EntityModificatorData _entityModificatorData;
    
    public override void Enable()
    {
        _entityModificatorData = Args.GetArgument<EntityModificatorData>("EntityModificatorData");
        
        _globalEnemyContainer.EnemyAdded += AddEntityModificator;
        _globalEnemyContainer.EnemyRemoved += RemoveEntityModificator;
        
        foreach (EnemyEntity enemyEntity in _globalEnemyContainer.Entities)
        {
            AddEntityModificator(enemyEntity);
        }
    }

    public override void Disable()
    {
        _globalEnemyContainer.EnemyAdded -= AddEntityModificator;
        _globalEnemyContainer.EnemyRemoved -= RemoveEntityModificator;
        
        foreach (EnemyEntity enemyEntity in _globalEnemyContainer.Entities)
        {
            RemoveEntityModificator(enemyEntity);
        }
    }

    private void AddEntityModificator(EnemyEntity entity)
    {
        entity.ComponentsContainer.Get<EntityModificatorsContainer>().AddEffect(_entityModificatorData);
    }

    private void RemoveEntityModificator(EnemyEntity entity)
    {
        entity.ComponentsContainer.Get<EntityModificatorsContainer>().RemoveEffect(_entityModificatorData);
    }
}