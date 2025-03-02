using Zenject;

public sealed class StartWaveToggleEffect : ToggleEffect
{
    [Inject] private WaveStateMachine _waveStateMachine;
    
    public override void Enable()
    {
        _waveStateMachine.TransitionToState(WaveState.Attack);
    }

    public override void Disable() {}
}