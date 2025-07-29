using Zenject;

public class ApplyModificatorOnWaveStartRemoveUponPickup : EntityModificator
{
    [Inject] private WaveStateMachine _waveStateMachine;
    
    public override void Enable()
    {
        Entity.Draggable.PickedUp += RemoveModificator;
        _waveStateMachine.StateStarted += TryToApplyModificator;
    }

    public override void Disable()
    {
        Entity.Draggable.PickedUp -= RemoveModificator;
        _waveStateMachine.StateStarted -= TryToApplyModificator;
        RemoveModificator();
    }

    private void TryToApplyModificator(WaveState waveState)
    {
        if (waveState != WaveState.Attack) return;
        
        Entity.ComponentsContainer.Get<EntityModificatorsContainer>().AddModificator(Args.GetArgument<EntityModificatorData>("EntityModificatorData"));
    }
    
    private void RemoveModificator()
    {
        Entity.ComponentsContainer.Get<EntityModificatorsContainer>().RemoveModificator(Args.GetArgument<EntityModificatorData>("EntityModificatorData"));
    }
}
