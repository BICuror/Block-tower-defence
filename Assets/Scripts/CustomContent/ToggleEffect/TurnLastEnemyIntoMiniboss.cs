using Zenject;
using Combat;

public sealed class TurnLastEnemyIntoMiniboss : GlobalToggleEffect
{
    [Inject] private EnemySpawnSystem _enemySpawnSystem;
    [Inject] private GlobalEnemyContainer _globalEnemyContainer;
    
    public override void Enable()
    {
        _globalEnemyContainer.EnemyRemoved += TryApplyModificator;
    }

    private void TryApplyModificator(EnemyEntity _)
    {
        if (_globalEnemyContainer.Entities.Count == 1 && _enemySpawnSystem.AllEnemiesSpawned)
        {
            EnemyEntity lastEnemy = _globalEnemyContainer.Entities[0];

            lastEnemy.ComponentsContainer.Get<EntityModificatorsContainer>().AddModificator(Args.GetArgument<EntityModificatorData>("EntityModificatorData"));
            lastEnemy.EnemyHealth.EnemyDied += RemoveEntityModificator;
            
            lastEnemy.Health.ReceivePercentHeal(1f);
        }
    }

    private void RemoveEntityModificator(EnemyEntity enemyEntity)
    {
        enemyEntity.ComponentsContainer.Get<EntityModificatorsContainer>().RemoveModificator(Args.GetArgument<EntityModificatorData>("EntityModificatorData"));

        enemyEntity.EnemyHealth.EnemyDied -= RemoveEntityModificator;
    }
    
    public override void Disable()
    {
        _globalEnemyContainer.EnemyRemoved -= TryApplyModificator;
    }
}
