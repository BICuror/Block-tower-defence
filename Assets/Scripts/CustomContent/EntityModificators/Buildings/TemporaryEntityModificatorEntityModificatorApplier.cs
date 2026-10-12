using Cysharp.Threading.Tasks;
using Combat.UpgradeCharges;
using Zenject;

public sealed class TemporaryEntityModificatorEntityModificatorApplier : EntityModificator
{
    [Inject] private UpgradeChargeContainer _upgradeChargeContainer;
    [Inject] private WaveStateMachine _waveStateMachine;
    private int _wavesLeft;
    
    public override void Enable()
    {
        _wavesLeft = Args.GetArgument<int>("WavesDuration");
        
        Entity.ComponentsContainer.Get<EntityModificatorsContainer>().AddEffect(Args.GetArgument<EntityModificatorData>("EntityModificator"));
        
        _waveStateMachine.GetWaveStateController(WaveState.Idle).EnteredStateStarted += TryRemoveModificator;
    }
    
    public override void Disable()
    {
        Entity.ComponentsContainer.Get<EntityModificatorsContainer>().RemoveModificator(Args.GetArgument<EntityModificatorData>("EntityModificator"));
        
        _waveStateMachine.GetWaveStateController(WaveState.Idle).EnteredStateStarted -= TryRemoveModificator;

        _upgradeChargeContainer.AddCharge(Entity.transform, Args.GetArgument<int>("UpgradeChargesAmount")).Forget();
    }

    private void TryRemoveModificator()
    {
        _wavesLeft--;
        
        if (_wavesLeft > 0) return;
        
        Entity.ComponentsContainer.Get<EntityModificatorsContainer>().RemoveModificator(ModificatorData);
    }
}