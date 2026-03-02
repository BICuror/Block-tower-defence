using UnityEngine;
using System;

public abstract class GameControllerState : MonoBehaviour
{
    protected GameControls GameControls;

    public event Action<ControllerState> TriedToEnterState;
    public event Action<ControllerState> TriedToExitState;

    protected bool IsActive;
    protected abstract ControllerState State { get; }
    
    public abstract void Initialize(GameControls controls);

    public void Enter()
    {
        IsActive = true;
        OnEnter();
    }

    public void Exit()
    {
        IsActive = false;
        OnExit();
    }
        
    public abstract bool CanEnterStateFrom(ControllerState currentState);
    public abstract bool CanExitStateTo(ControllerState currentState);

    protected virtual void OnEnter() {}
    protected virtual void OnExit() {}
    
    protected void InvokeTryEnterState() => TriedToEnterState?.Invoke(State);
    protected void InvokeTryExitState() => TriedToExitState?.Invoke(State);
}