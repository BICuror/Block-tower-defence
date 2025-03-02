using UnityEngine;
using Zenject;

public sealed class GlobalListInstaller : MonoInstaller
{
    [SerializeField] private GlobalBuildingContainer _globalBuildingContainer;
    [SerializeField] private GlobalEffectContainer _globalEffectContainer;
    
    public override void InstallBindings()
    {
        Container.Bind<GlobalBuildingContainer>().FromInstance(_globalBuildingContainer).AsSingle().NonLazy();
        Container.Bind<GlobalEffectContainer>().FromInstance(_globalEffectContainer).AsSingle().NonLazy();
    }
}