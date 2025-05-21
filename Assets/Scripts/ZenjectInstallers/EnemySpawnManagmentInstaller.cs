using UnityEngine;
using Zenject;
using Combat;

public sealed class EnemySpawnManagmentInstaller : MonoInstaller
{
    [SerializeField] private EnemySpawnGroupCompiler _enemySpawnGroupCompiler;
    [SerializeField] private EnemySpawnSystem _enemySpawnSystem;

    public override void InstallBindings()
    {
        Container.Bind<EnemySpawnGroupCompiler>().FromInstance(_enemySpawnGroupCompiler).AsSingle().NonLazy();
        Container.Bind<EnemySpawnSystem>().FromInstance(_enemySpawnSystem).AsSingle().NonLazy();
    }
}