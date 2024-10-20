using Zenject;
using UnityEngine;

public class OptionalTaskSystemInstaller : MonoInstaller
{
    [SerializeField] private OptionalTaskGenerator _optionalTaskGenerator;

    public override void InstallBindings()
    {
        Container.Bind<OptionalTaskGenerator>().FromInstance(_optionalTaskGenerator).AsSingle().NonLazy();
    }
}
