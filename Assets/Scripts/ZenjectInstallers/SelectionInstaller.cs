using UnityEngine;
using Zenject;

public sealed class SelectionInstaller : MonoInstaller
{
    [SerializeField] private CrystalManager _crystalManager;

    public override void InstallBindings()
    {
        Container.Bind<CrystalManager>().FromInstance(_crystalManager).AsSingle().NonLazy();
    }
}
