using Zenject;
using WorldGeneration;

public sealed class GenerationDataInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IslandGridHolder>().AsSingle().NonLazy();
        Container.Bind<IslandHeightMapHolder>().AsSingle().NonLazy();
        Container.Bind<RoadMapHolder>().AsSingle().NonLazy();
    }
}
