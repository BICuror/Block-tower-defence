using UnityEngine;
using Zenject;
using Combat;
using UnityEngine.Serialization;

public sealed class EnemySpawnManagmentInstaller : MonoInstaller
{
    [FormerlySerializedAs("_enemySpawnerSystem")] [SerializeField] private EnemySpawnSystem enemySpawnSystem;

    public override void InstallBindings()
    {
        Container.Bind<EnemySpawnSystem>().FromInstance(enemySpawnSystem).AsSingle().NonLazy();
    }
}
