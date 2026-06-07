using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;
using UnityEngine;
using Zenject;

public sealed class GameCameraPositionDragState : GameControllerState
{
    [Inject] private CameraRotationController _cameraRotationController;
    [Inject] private CameraPositionController _cameraPositionController;
    [Inject] private InspectorController _inspectorController;
    [Inject] private CursorController _cursorController;
    [Inject] private DragController _dragController;
    
    private InputAction _pointerPositionAction;
    private InputAction _cameraDragAction;

    protected override ControllerState State => ControllerState.PositionDragging;

    public override void Initialize(InputActionMap actionMap)
    {
        _pointerPositionAction = actionMap["PointerPosition"];
        _cameraDragAction = actionMap["ActivateOrDragCamera"];
        
        _cameraPositionController.CameraPositionUpdated += _cameraRotationController.UpdateCameraRotation;
        _cameraDragAction.canceled += _ => InvokeTryExitState();
        _cameraDragAction.started += _ => TryEnterState();
    }

    private void TryEnterState()
    {
        if (_inspectorController.HoveredOverInspectable(GetPointerPosition())) return;
        
        if (_dragController.HoveredOverActivatable(GetPointerPosition())) return;
        
        if (_inspectorController.IsHoveredOverNonIdleUI()) return;

        InvokeTryEnterState();
    }

    protected override void OnEnter()
    {
        _cameraPositionController.CaptureCameraPosition(GetPointerPosition());

        RepositionCamera().Forget();
        
        _cursorController.SetCursorState(CursorController.CursorState.Move);
    }

    private async UniTask RepositionCamera()
    {
        while (_cameraDragAction.IsPressed() && IsActive)
        {
            await UniTask.WaitForFixedUpdate();

            _cameraPositionController.DragCamera(GetPointerPosition());
        }
        
        InvokeTryExitState();
    }

    public override bool CanEnterStateFrom(ControllerState currentState) => currentState is ControllerState.Idle or ControllerState.Rotating or ControllerState.Inspecting;

    public override bool CanExitStateTo(ControllerState currentState) => currentState is ControllerState.Idle or ControllerState.Rotating or ControllerState.Dragging;
    
    private Vector2 GetPointerPosition() => _pointerPositionAction.ReadValue<Vector2>();
}