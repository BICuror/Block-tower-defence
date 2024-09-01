using Zenject;
using UnityEngine;
using Navigation;

public sealed class NavigationSystemInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<NavigationMapHolder>().AsSingle().NonLazy();
    }
}
