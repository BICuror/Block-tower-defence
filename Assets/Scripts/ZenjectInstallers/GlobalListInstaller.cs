using Zenject;

public sealed class GlobalListInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<GlobalBuildingContainer>().AsSingle().NonLazy();
        Container.Bind<GlobalEffectContainer>().AsSingle().NonLazy();
        Container.Bind<GlobalEnemyContainer>().AsSingle().NonLazy();
        Container.Bind<GlobalStatContainer>().AsSingle().NonLazy();
    }
}