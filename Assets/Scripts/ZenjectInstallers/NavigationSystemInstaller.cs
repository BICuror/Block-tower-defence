using Zenject;
using UnityEngine;
using Navigation;

public sealed class NavigationSystemInstaller : MonoInstaller
{
    [SerializeField] private NavigationNodeMapGenerator _navigationNodeMapGenerator;
    [SerializeField] private NavigationMapGenerator _navigationMapGenerator;
    [SerializeField] private NavigationMapHolder _navigationMapHolder;
    [SerializeField] private NavigationMapper _navigationMapper;
    
    public override void InstallBindings()
    {
        Container.Bind<NavigationNodeMapGenerator>().FromInstance(_navigationNodeMapGenerator).AsSingle().NonLazy();
        Container.Bind<NavigationMapGenerator>().FromInstance(_navigationMapGenerator).AsSingle().NonLazy();
        Container.Bind<NavigationMapHolder>().FromInstance(_navigationMapHolder).AsSingle().NonLazy();
        Container.Bind<NavigationMapper>().FromInstance(_navigationMapper).AsSingle().NonLazy();
    }
}