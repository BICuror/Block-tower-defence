using UnityEngine;
using Zenject;
using Combat;

public sealed class GlobalEnemyModificatorToggleEffect : GlobalToggleEffect
{
    [Inject] private GlobalEnemyContainer _globalEnemyContainer;
    private EntityModificatorData _entityModificatorData;
    private int _chance = 100;
    
    public override void Enable()
    {
        if (Args.HasArgument("Chance")) _chance = Args.GetArgument<int>("Chance");
        
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
        if (Random.Range(0, 100) <= _chance)
        {
            entity.ComponentsContainer.Get<EntityModificatorsContainer>().AddModificator(_entityModificatorData);
        }
    }

    private void RemoveEntityModificator(EnemyEntity entity)
    {
        entity.ComponentsContainer.Get<EntityModificatorsContainer>().RemoveModificator(_entityModificatorData);
    }
}