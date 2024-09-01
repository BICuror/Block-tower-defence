using UnityEngine;
using Zenject;
using WorldGeneration;

public class RoadGenerationInstaller : MonoInstaller
{
    [SerializeField] private RoadMapGenerator _roadMapGenerator;
    [SerializeField] private RoadGenerator _roadGenerator;
    [SerializeField] private EnemyNavigator _enemyNavigator;

    public override void InstallBindings()
    {
        Container.Bind<RoadMapGenerator>().FromInstance(_roadMapGenerator).AsSingle().NonLazy();
        Container.Bind<RoadGenerator>().FromInstance(_roadGenerator).AsSingle().NonLazy();

        Container.Bind<EnemyNavigator>().FromInstance(_enemyNavigator).AsSingle().NonLazy();
        
        Container.Bind<RoadNodeGenerator>().AsSingle().NonLazy();
        Container.Bind<SpawnerRoadNodeGenerator>().AsSingle().NonLazy();
    }
}
