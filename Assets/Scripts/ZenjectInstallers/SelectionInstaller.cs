using UnityEngine;
using Zenject;

public sealed class SelectionInstaller : MonoInstaller
{
    [SerializeField] private ItemFactory _itemFactory;
    [SerializeField] private ItemContainerManager _itemContainerManager;
    [SerializeField] private SelectionManager _selectionManager;
    
    public override void InstallBindings()
    {
        Container.Bind<SelectionManager>().FromInstance(_selectionManager).AsSingle();
        Container.Bind<ItemContainerManager>().FromInstance(_itemContainerManager).AsSingle();
        Container.Bind<ItemFactory>().FromInstance(_itemFactory).AsSingle();
        Container.Bind<EffectFactory>().AsSingle();
    }
}
