using Zenject;
using UnityEngine;
using Navigation;

public sealed class NavigationSystemInstaller : MonoInstaller
{
    [SerializeField] private DefaultNavigationMapGenerator _defaultNavigationMapGenerator;
    [SerializeField] private OptionalNavigationMapGenerator _optionalNavigationMapGenerator;
    [SerializeField] private NavigationMapGenerator _navigationMapGenerator;

 
    public override void InstallBindings()
    {
        Container.Bind<NavigationMapHolder>().AsSingle().NonLazy();

        Container.Bind<DefaultNavigationMapGenerator>().FromInstance(_defaultNavigationMapGenerator).AsSingle().NonLazy();
        Container.Bind<OptionalNavigationMapGenerator>().FromInstance(_optionalNavigationMapGenerator).AsSingle().NonLazy();
        Container.Bind<NavigationMapGenerator>().FromInstance(_navigationMapGenerator).AsSingle().NonLazy();
    }
}
