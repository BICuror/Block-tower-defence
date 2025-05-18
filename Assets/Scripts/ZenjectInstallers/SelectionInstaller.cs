using UnityEngine;
using Zenject;

public sealed class SelectionInstaller : MonoInstaller
{
    [SerializeField] private SelectionManager _selectionManager;
    
    public override void InstallBindings()
    {
        Container.Bind<SelectionManager>().FromInstance(_selectionManager).AsSingle();
        Container.Bind<GlobalEffectFactory>().AsSingle();
        Container.Bind<EntityModificatorFactory>().AsSingle();
    }
}