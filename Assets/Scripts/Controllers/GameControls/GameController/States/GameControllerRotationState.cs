using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;
using UnityEngine;
using Zenject;

public sealed class GameControllerRotationState : GameControllerState
{
    [Inject] private CameraRotationController _cameraRotationController;
    [Inject] private InspectorController _inspectorController;
    
    private InputAction _pointerPositionAction;
    private InputAction _dragAction;
    
    protected override ControllerState State => ControllerState.Rotating;
    
    public override void Initialize(InputActionMap actionMap)
    {
        _pointerPositionAction = actionMap["PointerPosition"];
        _dragAction = actionMap["DragOrRotateCamera"];
        
        _dragAction.started += _ => TryEnterState();
        _dragAction.canceled += _ => InvokeTryExitState();
    }
    
    private void TryEnterState()
    {
        if (_inspectorController.IsHoveredOverNonIdleUI()) return;

        InvokeTryEnterState();
    }

    protected override void OnEnter()
    {
        _cameraRotationController.SetPreviousMousePosition(GetPointerPosition());
        RotateCamera().Forget();
    }

    private async UniTask RotateCamera()
    {
        while (_dragAction.IsPressed() && IsActive)
        {
            await UniTask.WaitForFixedUpdate();
            
            _cameraRotationController.Rotate(GetPointerPosition());
        }
        
        InvokeTryExitState();
    }

    public override bool CanEnterStateFrom(ControllerState currentState) => currentState is ControllerState.Idle or ControllerState.PositionDragging or ControllerState.Inspecting;

    public override bool CanExitStateTo(ControllerState currentState) => currentState is ControllerState.Inspecting or ControllerState.Idle or ControllerState.PositionDragging; 
    
    private Vector2 GetPointerPosition() => _pointerPositionAction.ReadValue<Vector2>();
}