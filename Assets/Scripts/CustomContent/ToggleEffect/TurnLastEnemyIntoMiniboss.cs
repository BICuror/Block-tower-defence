using Zenject;
using Combat;

public sealed class TurnLastEnemyIntoMiniboss : GlobalToggleEffect
{
    [Inject] private EnemySpawnSystem _enemySpawnSystem;
    [Inject] private GlobalEnemyContainer _globalEnemyContainer;
    private EntityEffectParticleHandler _instantiatedParticleHandler;
    
    public override void Enable()
    {
        _globalEnemyContainer.EnemyRemoved += TryApplyModificator;
    }

    private void TryApplyModificator(EnemyEntity _)
    {
        if (_globalEnemyContainer.Entities.Count == 1 && _enemySpawnSystem.IsAllEnemiesSpawned())
        {
            EnemyEntity lastEnemy = _globalEnemyContainer.Entities[0];

            lastEnemy.ComponentsContainer.Get<EntityModificatorsContainer>().AddEffect(Args.GetArgument<EntityModificatorData>("EntityModificatorData"));
            lastEnemy.EnemyHealth.EnemyDied += RemoveEntityModificator;
            
            lastEnemy.Health.ReceivePercentHeal(1f);
            
            _instantiatedParticleHandler = lastEnemy.ComponentsContainer.Get<ParticleEffectManager>().ApplyCustomEffect(Args.GetArgument<EntityEffectParticleHandler>("EntityEffectParticleHandler"));
        }
    }

    private void RemoveEntityModificator(EnemyEntity enemyEntity)
    {
        enemyEntity.ComponentsContainer.Get<ParticleEffectManager>().DestroyCustomEffect(_instantiatedParticleHandler);
        
        enemyEntity.ComponentsContainer.Get<EntityModificatorsContainer>().RemoveEffect(Args.GetArgument<EntityModificatorData>("EntityModificatorData"));

        enemyEntity.EnemyHealth.EnemyDied -= RemoveEntityModificator;
    }
    
    public override void Disable()
    {
        _globalEnemyContainer.EnemyRemoved -= TryApplyModificator;
    }
}
