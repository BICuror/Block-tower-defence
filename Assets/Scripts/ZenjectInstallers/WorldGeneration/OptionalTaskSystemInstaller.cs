using Combat.OptionalTasks;
using UnityEngine;
using Zenject;

public sealed class OptionalTaskSystemInstaller : MonoInstaller
{
    [SerializeField] private OptionalTaskGenerationController _optionalTaskGenerationController;
    
    public override void InstallBindings()
    {
        Container.Bind<OptionalTaskGenerationController>().FromInstance(_optionalTaskGenerationController).AsSingle().NonLazy();
        Container.Bind<OptionalTaskObjectsManager>().AsSingle().NonLazy();
    }
}