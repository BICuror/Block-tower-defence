using UnityEngine;
using Zenject;

public sealed class DraggableLogicInstaller : MonoInstaller
{
    [SerializeField] private DraggableSystemConfig _draggableSystemConfig;
    [SerializeField] private DraggableCreator _draggableCreator;

    public override void InstallBindings()
    {
        Container.Bind<DraggableCreator>().FromInstance(_draggableCreator).AsSingle().NonLazy();
        Container.Bind<DraggableSystemConfig>().FromInstance(_draggableSystemConfig);
    }
}
