using UnityEngine.InputSystem;
using UnityEngine;
using System;

namespace GameControls.States
{
    public abstract class GameControllerState : MonoBehaviour
    {
        protected InputActionMap InputActionMap;
        protected bool IsActive;
        
        protected abstract ControllerState State { get; }
        
        public event Action<ControllerState> TriedToEnterState;
        public event Action<ControllerState> TriedToExitState;
        
        public void SetInputActionMap(InputActionMap actionMap) => InputActionMap = actionMap;
        public abstract void Initialize();
        
        public void EnableState()
        {
            enabled = true;
            OnEnableState();
        }

        public void DisableState()
        {
            enabled = false;
            OnDisableState();
        }
        
        protected abstract void OnEnableState();
        protected abstract void OnDisableState();
        
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
        
        protected void InvokeTryEnterState(InputAction.CallbackContext _) => InvokeTryEnterState();
        protected void InvokeTryExitState(InputAction.CallbackContext _) => InvokeTryExitState();
        protected void InvokeTryEnterState() => TriedToEnterState?.Invoke(State);
        protected void InvokeTryExitState() => TriedToExitState?.Invoke(State);
    }
}