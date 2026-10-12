using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Combat.EnemySpawnSystem;
using UnityEngine;
using Zenject;
using System;

public sealed class WaveStateMachine : MonoBehaviour
{
    private const WaveState INITIAL_STATE = WaveState.None;

    [Inject] private EnemySpawnSystemController _enemySpawnSystem;
    [SerializeField] private List<WaveStateController> _stateControllersList;
    private Dictionary<WaveState, WaveStateController> _stateControllers = new();
    private WaveState _currentState;

    public WaveState CurrentState => _currentState;
    
    public event Action<WaveState> StateStarted;
    public event Action<WaveState> StateEnded;
    
    public void Initialize()
    {
        _enemySpawnSystem.WaveComplete += TransitionIntoIdle;
        
        _currentState = INITIAL_STATE;

        _stateControllersList.ForEach(controller => 
        {
            _stateControllers.Add(controller.GetControlledState(), controller);
        });
    }

    public WaveStateController GetWaveStateController(WaveState waveState) => _stateControllers[waveState];
    
    public void TransitionIntoIdle() => TransitionOutToState(WaveState.Idle); 
    public void TransitionIntoAttack() => TransitionOutToState(WaveState.Attack); 

    public void TransitionToState(WaveState waveState)
    {
        TransitionOutToState(waveState);
    }
    
    private async void TransitionOutToState(WaveState stateToTransitionTo)
    {
        if (_currentState != WaveState.None) await TransitionOutOfCurrentState();

        _currentState = stateToTransitionTo;

        await TransitionIntoNewState();
    }

    private async UniTask TransitionOutOfCurrentState()
    {
        await _stateControllers[_currentState].TransitionOutOfState();
        StateEnded?.Invoke(_currentState);
    }

    private async UniTask TransitionIntoNewState()
    {
        await _stateControllers[_currentState].TransitionIntoState();
        StateStarted?.Invoke(_currentState);
    } 
}