using Combat.Entity.Modificators;
using Zenject;

public class ApplyModificatorOnIdleStartRemoveUponPickup : EntityModificator
{
    [Inject] private WaveStateMachine _waveStateMachine;
    
    public override void Enable()
    {
        Entity.ComponentsContainer.Get<DraggableObject>().PickedUp += TryRemoveModificator;
        _waveStateMachine.StateStarted += TryToApplyModificator;
        _waveStateMachine.StateEnded += TryRemoveModificator;
        TryToApplyModificator(_waveStateMachine.CurrentState);
    }

    public override void Disable()
    {
        Entity.ComponentsContainer.Get<DraggableObject>().PickedUp -= TryRemoveModificator;
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
        if (!Entity.ComponentsContainer.Get<EntityModificatorsContainer>().Has(Args.GetArgument<EntityModificatorConfig>("EntityModificatorConfig")))
        { 
            Entity.ComponentsContainer.Get<EntityModificatorsContainer>().AddModificator(Args.GetArgument<EntityModificatorConfig>("EntityModificatorConfig"));
        }
    }
    
    private void RemoveModificator()
    {
        if (Entity.ComponentsContainer.Get<EntityModificatorsContainer>().Has(Args.GetArgument<EntityModificatorConfig>("EntityModificatorConfig")))
        { 
            Entity.ComponentsContainer.Get<EntityModificatorsContainer>().RemoveModificator(Args.GetArgument<EntityModificatorConfig>("EntityModificatorConfig"));
        }
    }
}
