using UnityEngine;
using Zenject;

public sealed class ItemManagementInstaller : MonoInstaller
{
    [SerializeField] private ItemsContainer _itemsContainer;
    [SerializeField] private ItemFactory _itemFactory;
    [SerializeField] private ItemContainerManager _itemContainerManager;
    
    public override void InstallBindings()
    {
        Container.Bind<ItemsContainer>().FromInstance(_itemsContainer).AsSingle();
        Container.Bind<ItemContainerManager>().FromInstance(_itemContainerManager).AsSingle();
        Container.Bind<ItemFactory>().FromInstance(_itemFactory).AsSingle();
    }
}