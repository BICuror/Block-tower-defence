using UnityEngine;
using Zenject;

public sealed class EnemySpawnManagmentInstaller : MonoInstaller
{
    [SerializeField] private EnemySpawnerSystem _enemySpawnerSystem;

    public override void InstallBindings()
    {
        Container.Bind<EnemySpawnerSystem>().FromInstance(_enemySpawnerSystem).AsSingle().NonLazy();
    }
}
