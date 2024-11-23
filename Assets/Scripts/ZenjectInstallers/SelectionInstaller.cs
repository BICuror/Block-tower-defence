using UnityEngine;
using Zenject;

public sealed class SelectionInstaller : MonoInstaller
{
    [SerializeField] private ItemContainerManager _itemContainerManager;

    public override void InstallBindings()
    {
        Container.Bind<ItemContainerManager>().FromInstance(_itemContainerManager).AsSingle().NonLazy();
    }
}
