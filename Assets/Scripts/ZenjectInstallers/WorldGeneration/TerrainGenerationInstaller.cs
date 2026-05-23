using UnityEngine;
using UnityEngine.Serialization;
using Zenject;
using WorldGeneration;

namespace Combat
{
    public sealed class TerrainGenerationInstaller : MonoInstaller
    {
        [SerializeField] private IslandDecorationGenerator _decorationGenerator;
        [SerializeField] private EnviromentCreator _enviromentCreator;
        [SerializeField] private WaveIndexContainer _instance; 
        [SerializeField] private IslandTerrainMeshCreator _islandTerrainMeshCreator;
        [SerializeField] private IslandDecorationContainer _islandDecorationContainer;
        [SerializeField] private WaveStateMachine _waveStateMachine;
    
        public override void InstallBindings()
        {
            Container.Bind<BiomeMapGenerator>().AsSingle().NonLazy();
            Container.Bind<HeightMapGenerator>().AsSingle().NonLazy();
    
            Container.Bind<WaveStateMachine>().FromInstance(_waveStateMachine).AsSingle().NonLazy();
            Container.Bind<IslandDecorationGenerator>().FromInstance(_decorationGenerator).AsSingle().NonLazy();
            Container.Bind<IslandDecorationContainer>().FromInstance(_islandDecorationContainer).AsSingle().NonLazy();
            Container.Bind<IslandTerrainMeshCreator>().FromInstance(_islandTerrainMeshCreator).AsSingle().NonLazy();
            Container.Bind<EnviromentCreator>().FromInstance(_enviromentCreator).AsSingle().NonLazy();
            Container.Bind<WaveIndexContainer>().FromInstance(_instance).AsSingle().NonLazy();
        }
    }
}

