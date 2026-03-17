using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class GameControllerIdleState : GameControllerState
{
    [SerializeField] private InspectorController _inspectorController;
    [SerializeField] private CursorController _cursorController;
    private GameControls _controls;

    protected override ControllerState State => ControllerState.Idle;

    public override void Initialize(GameControls controls)
    {
        _controls = controls;
    }

    protected override void OnEnter()
    {
        _cursorController.SetCursorState(CursorController.CursorState.Idle);
        
        ChangeCursorHoverState().Forget();
    }

    private async UniTask ChangeCursorHoverState()
    {
        while (IsActive)
        {
            bool hoveredOverInspectable = _inspectorController.HoveredOverInspectable(GetPointerPosition());
            
            if (hoveredOverInspectable && _cursorController.State != CursorController.CursorState.InspectionAvailable)
            {
                _cursorController.SetCursorState(CursorController.CursorState.InspectionAvailable);
            }
            else if (!hoveredOverInspectable && _cursorController.State != CursorController.CursorState.Idle)
            {
                _cursorController.SetCursorState(CursorController.CursorState.Idle);
            }

            await UniTask.WaitForFixedUpdate();
        }
    }
    
    public override bool CanEnterStateFrom(ControllerState currentState) => true;
    public override bool CanExitStateTo(ControllerState currentState) => true;
    
    private Vector2 GetPointerPosition() => _controls.TouchInput.PointerPosition.ReadValue<Vector2>();
}