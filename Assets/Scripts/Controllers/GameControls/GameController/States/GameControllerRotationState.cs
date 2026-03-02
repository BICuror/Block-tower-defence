using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class GameControllerRotationState : GameControllerState
{
    [SerializeField] private CameraRotationController _cameraRotationController;
    private GameControls _controls;
    
    protected override ControllerState State => ControllerState.Rotating;
    
    public override void Initialize(GameControls controls)
    {
        _controls = controls;
        
        _controls.TouchInput.LMB.started += _ => InvokeTryEnterState();
        _controls.TouchInput.LMB.canceled += _ => InvokeTryExitState();
    }

    protected override void OnEnter()
    {
        _cameraRotationController.SetPreviousMousePosition(GetPointerPosition());
        RotateCamera().Forget();
    }

    private async UniTask RotateCamera()
    {
        while (_controls.TouchInput.LMB.IsPressed() && IsActive)
        {
            await UniTask.WaitForFixedUpdate();
            
            _cameraRotationController.Rotate(GetPointerPosition());
        }
        
        InvokeTryExitState();
    }

    public override bool CanEnterStateFrom(ControllerState currentState) => currentState is ControllerState.Idle or ControllerState.PositionDragging;

    public override bool CanExitStateTo(ControllerState currentState) => currentState is ControllerState.Inspecting or ControllerState.Idle or ControllerState.PositionDragging; 
    
    private Vector2 GetPointerPosition() => _controls.TouchInput.PointerPosition.ReadValue<Vector2>();
}