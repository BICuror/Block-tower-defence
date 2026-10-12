using Zenject;
using Combat;
using Combat.EnemySpawnSystem;

public sealed class TurnLastEnemyIntoMiniboss : GlobalEffect
{
    [Inject] private GlobalEnemyContainer _globalEnemyContainer;
    [Inject] private EnemySpawnSystemController _enemySpawnSystem;
    
    public override void Enable()
    {
        _globalEnemyContainer.EnemyRemoved += TryApplyModificator;
    }

    private void TryApplyModificator(EnemyEntity _)
    {
        if (_globalEnemyContainer.Entities.Count == 1 && _enemySpawnSystem.AllEnemiesSpawned)
        {
            EnemyEntity lastEnemy = _globalEnemyContainer.Entities[0];

            lastEnemy.ComponentsContainer.Get<EntityModificatorsContainer>().AddEffect(Args.GetArgument<EntityModificatorData>("EntityModificatorData"));
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
