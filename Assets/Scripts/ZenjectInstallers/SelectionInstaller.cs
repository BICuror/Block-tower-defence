using UnityEngine;
using Zenject;

public sealed class SelectionInstaller : MonoInstaller
{
    [SerializeField] private UpgradeChargeContainer _upgradeChargeContainer;
    [SerializeField] private SelectionManager _selectionManager;
    
    public override void InstallBindings()
    {
        Container.Bind<UpgradeChargeContainer>().FromInstance(_upgradeChargeContainer).AsSingle();
        Container.Bind<SelectionManager>().FromInstance(_selectionManager).AsSingle();
        Container.Bind<GlobalEffectFactory>().AsSingle();
        Container.Bind<EntityModificatorFactory>().AsSingle();
    }
}