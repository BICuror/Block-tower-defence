using EventLinks;
using Zenject;
using System;

public sealed class OnWaveStartEventLink : ParameterlessEventLink
{
    [Inject] private WaveStateMachine _waveStateMachine;
    
    protected override void SubscribeToEvent(Action action)
    {
        _waveStateMachine.GetWaveStateController(WaveState.Attack).EnteredStateStarted += action;
    }

    protected override void UnsubscribeToEvent(Action action)
    {
        _waveStateMachine.GetWaveStateController(WaveState.Attack).EnteredStateStarted -= action;
    }
}