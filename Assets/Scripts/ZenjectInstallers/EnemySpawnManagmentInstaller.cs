using Combat.EnemySpawnSystem;
using UnityEngine;
using Zenject;

public sealed class EnemySpawnManagmentInstaller : MonoInstaller
{
    [SerializeField] private EnemySpawnGroupCompiler _enemySpawnGroupCompiler;
    [SerializeField] private EnemySpawnSystemController _enemySpawnSystem;

    public override void InstallBindings()
    {
        Container.Bind<EnemySpawnGroupCompiler>().FromInstance(_enemySpawnGroupCompiler).AsSingle().NonLazy();
        Container.Bind<EnemySpawnSystemController>().FromInstance(_enemySpawnSystem).AsSingle().NonLazy();
        Container.Bind<EnemySpawnGroupContainer>().AsSingle().NonLazy();
    }
}