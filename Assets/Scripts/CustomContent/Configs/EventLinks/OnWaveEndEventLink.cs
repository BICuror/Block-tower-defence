using EventLinks;
using Zenject;
using System;

public sealed class OnWaveEndEventLink : ParameterlessEventLink
{
    [Inject] private WaveStateMachine _waveStateMachine;
    
    protected override void SubscribeToEvent(Action action)
    {
        _waveStateMachine.GetWaveStateController(WaveState.Attack).QuitStateCompleted += action;
    }

    protected override void UnsubscribeToEvent(Action action)
    {
        _waveStateMachine.GetWaveStateController(WaveState.Attack).QuitStateCompleted -= action;
    }
}