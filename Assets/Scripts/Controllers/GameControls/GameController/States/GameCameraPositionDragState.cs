using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class GameCameraPositionDragState : GameControllerState
{
    [SerializeField] private CameraRotationController _cameraRotationController;
    [SerializeField] private CameraPositionController _cameraPositionController;
    [SerializeField] private InspectorController _inspectorController;
    [SerializeField] private DragController _dragController;
    private GameControls _controls;

    protected override ControllerState State => ControllerState.PositionDragging;

    public override void Initialize(GameControls controls)
    {
        _controls = controls;

        _cameraPositionController.CameraPositionUpdated += _cameraRotationController.UpdateCameraRotation;
        _controls.TouchInput.RMB.canceled += _ => InvokeTryExitState();
        _controls.TouchInput.RMB.started += _ => TryEnterState();
    }

    private void TryEnterState()
    {
        if (_inspectorController.HoveredOverInspectable(GetPointerPosition())) return;

        if (_dragController.HoveredOverActivatable(GetPointerPosition())) return;

        InvokeTryEnterState();
    }

    protected override void OnEnter()
    {
        _cameraPositionController.CaptureCameraPosition(GetPointerPosition());

        RepositionCamera().Forget();
    }

    private async UniTask RepositionCamera()
    {
        while (_controls.TouchInput.RMB.IsPressed() && IsActive)
        {
            await UniTask.WaitForFixedUpdate();

            _cameraPositionController.DragCamera(GetPointerPosition());
        }
        
        InvokeTryExitState();
    }

    public override bool CanEnterStateFrom(ControllerState currentState) => currentState is ControllerState.Idle or ControllerState.Rotating or ControllerState.Inspecting;

    public override bool CanExitStateTo(ControllerState currentState) => currentState is ControllerState.Idle or ControllerState.Rotating or ControllerState.Dragging;
    
    private Vector2 GetPointerPosition() => _controls.TouchInput.PointerPosition.ReadValue<Vector2>();
}