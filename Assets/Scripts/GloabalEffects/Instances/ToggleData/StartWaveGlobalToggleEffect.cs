using Zenject;

public sealed class StartWaveGlobalToggleEffect : GlobalToggleEffect
{
    [Inject] private WaveStateMachine _waveStateMachine;
    
    public override void Enable()
    {
        _waveStateMachine.TransitionToState(WaveState.Attack);
    }

    public override void Disable() {}
}