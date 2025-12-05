using Zenject;

public class ApplyModificatorOnIdleStartRemoveUponPickup : EntityModificator
{
    [Inject] private WaveStateMachine _waveStateMachine;
    
    public override void Enable()
    {
        Entity.Draggable.PickedUp += TryRemoveModificator;
        _waveStateMachine.StateStarted += TryToApplyModificator;
        _waveStateMachine.StateEnded += TryRemoveModificator;
        TryToApplyModificator(_waveStateMachine.CurrentState);
    }

    public override void Disable()
    {
        Entity.Draggable.PickedUp -= TryRemoveModificator;
        _waveStateMachine.StateStarted -= TryToApplyModificator;
        _waveStateMachine.StateEnded -= TryRemoveModificator;
        RemoveModificator();
    }

    private void TryToApplyModificator(WaveState waveState)
    {
        if (waveState == WaveState.Idle)
        {
            ApplyModificator();
        }
    }
    
    private void TryRemoveModificator()
    {
        TryRemoveModificator(_waveStateMachine.CurrentState);
        
    }
    private void TryRemoveModificator(WaveState waveState)
    {
        if (waveState == WaveState.Attack)
        {
            RemoveModificator();
        }
    }   
    
    private void ApplyModificator()
    {
        if (!Entity.ComponentsContainer.Get<EntityModificatorsContainer>().Has(Args.GetArgument<EntityModificatorData>("EntityModificatorData")))
        { 
            Entity.ComponentsContainer.Get<EntityModificatorsContainer>().AddModificator(Args.GetArgument<EntityModificatorData>("EntityModificatorData"));
        }
    }
    
    private void RemoveModificator()
    {
        if (Entity.ComponentsContainer.Get<EntityModificatorsContainer>().Has(Args.GetArgument<EntityModificatorData>("EntityModificatorData")))
        { 
            Entity.ComponentsContainer.Get<EntityModificatorsContainer>().RemoveModificator(Args.GetArgument<EntityModificatorData>("EntityModificatorData"));
        }
    }
}
