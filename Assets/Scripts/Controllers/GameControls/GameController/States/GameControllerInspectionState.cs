using UnityEngine.InputSystem.Interactions;
using UnityEngine.InputSystem;
using UnityEngine;
using Zenject;

public sealed class GameControllerInspectionState : GameControllerState
{
    [Inject] private InspectorController _inspectorController;
    private GameControls _controls;

    protected override ControllerState State => ControllerState.Inspecting;
    
    public override void Initialize(GameControls controls)
    {
        _controls = controls;
        
        _controls.TouchInput.RMB.performed += TryEnterState;
        _inspectorController.InspectionStopped += InvokeTryExitState;
    }

    private void TryEnterState(InputAction.CallbackContext context)
    {
        if (context.interaction is TapInteraction)
        {
            _inspectorController.StopInspecting();
        }
        else if (context.interaction is HoldInteraction)
        {
            if (_inspectorController.HoveredOverInspectable(GetPointerPosition()))
            {
                InvokeTryEnterState();
            }
        }
    }

    protected override void OnEnter()
    {
        _inspectorController.TryToStartInspecting(GetPointerPosition());
    }

    protected override void OnExit()
    {
        _inspectorController.StopInspecting();
    }

    public override bool CanEnterStateFrom(ControllerState currentState) => currentState is ControllerState.Idle or ControllerState.Inspecting;

    public override bool CanExitStateTo(ControllerState currentState) => currentState is ControllerState.Idle or ControllerState.Inspecting or ControllerState.Dragging;
    
    private void FixedUpdate() 
    { 
        if (_inspectorController.TryToStartIdleInspecting(GetPointerPosition())) InvokeTryEnterState();
    }
    
    private Vector2 GetPointerPosition() => _controls.TouchInput.PointerPosition.ReadValue<Vector2>();
}
