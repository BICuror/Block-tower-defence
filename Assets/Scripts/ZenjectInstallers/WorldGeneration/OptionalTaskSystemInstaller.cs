using Zenject;
using UnityEngine;
using UnityEngine.Serialization;

public class OptionalTaskSystemInstaller : MonoInstaller
{
    [FormerlySerializedAs("_optionalTaskGenerator")] [SerializeField] private OptionalTaskManager optionalTaskManager;

    public override void InstallBindings()
    {
        Container.Bind<OptionalTaskManager>().FromInstance(optionalTaskManager).AsSingle().NonLazy();
    }
}
