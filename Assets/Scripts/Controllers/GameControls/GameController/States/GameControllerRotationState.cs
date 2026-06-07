using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;
using UnityEngine;
using Zenject;

public sealed class GameControllerRotationState : GameControllerState
{
    [Inject] private CameraRotationController _cameraRotationController;
    [Inject] private InspectorController _inspectorController;
    
    private InputAction _pointerDeltaAction;
    private InputAction _dragAction;
    
    protected override ControllerState State => ControllerState.Rotating;
    
    public override void Initialize(InputActionMap actionMap)
    {
        _pointerDeltaAction = actionMap["PointerDelta"];
        _dragAction = actionMap["DragOrRotateCamera"];
        
        _dragAction.canceled += InvokeTryExitState;
        _dragAction.started += TryEnterState;
    }
    
    private void TryEnterState(InputAction.CallbackContext _)
    {
        if (_inspectorController.IsHoveredOverNonIdleUI()) return;

        InvokeTryEnterState();
    }

    protected override void OnEnter()
    {
        RotateCamera().Forget();
    }

    private async UniTask RotateCamera()
    {
        while (_dragAction.IsPressed() && IsActive)
        {
            await UniTask.WaitForFixedUpdate();
            
            _cameraRotationController.Rotate(GetPointerDelta());
        }
        
        InvokeTryExitState();
    }

    public override bool CanEnterStateFrom(ControllerState currentState) => currentState is ControllerState.Idle or ControllerState.PositionDragging or ControllerState.Inspecting;

    public override bool CanExitStateTo(ControllerState currentState) => currentState is ControllerState.Inspecting or ControllerState.Idle or ControllerState.PositionDragging; 
    
    private Vector2 GetPointerDelta() => _pointerDeltaAction.ReadValue<Vector2>() / Time.timeScale;

    public override void UnbindInputActions()
    {
        _dragAction.canceled -= InvokeTryExitState;
        _dragAction.started -= TryEnterState;
    }
}