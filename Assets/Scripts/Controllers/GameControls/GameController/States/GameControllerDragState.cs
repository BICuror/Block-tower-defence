using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public sealed class GameControllerDragState : GameControllerState
{
    [Inject] private InspectorController _inspectorController;
    [Inject] private CursorController _cursorController;
    [Inject] private DragController _dragController;
    
    private InputAction _pointerPositionAction;
    private InputAction _activateAction;
    private InputAction _dragAction;
    
    protected override ControllerState State => ControllerState.Dragging;

    public override void Initialize(InputActionMap actionMap)
    {
        _pointerPositionAction = actionMap["PointerPosition"];
        _activateAction = actionMap["ActivateOrDragCamera"];
        _dragAction = actionMap["DragOrRotateCamera"];
        
        _dragAction.started += _ => TryEnterState();
        _dragAction.canceled += _ => InvokeTryExitState();
        _activateAction.performed += TryActivateObject;
    }

    private void TryActivateObject(InputAction.CallbackContext context)
    { 
        _dragController.ActivatedSomething(GetPointerPosition());
    }

    private void TryEnterState()
    {
        if (_dragController.HoveredOverDraggableObject(GetPointerPosition(), out GameObject draggedObject))
        {
            if (!_inspectorController.IsPossibleToDragInspectedItem(draggedObject)) return;

            InvokeTryEnterState();
        }
    }

    protected override void OnEnter()
    {
        _dragController.PickUpDraggable(GetPointerPosition());
        
        DragObject().Forget();
        
        _cursorController.SetCursorState(CursorController.CursorState.Drag);
    }

    private async UniTask DragObject()
    {
        while (_dragAction.IsPressed() && IsActive)
        {
            await UniTask.WaitForFixedUpdate();
            
            _dragController.TryDragTo(GetPointerPosition());
        }
        
        _dragController.DropDraggable(GetPointerPosition());
        InvokeTryExitState();
    }
    
    public override bool CanEnterStateFrom(ControllerState currentState) => currentState is ControllerState.Idle or ControllerState.Inspecting or ControllerState.PositionDragging;

    public override bool CanExitStateTo(ControllerState currentState) => currentState is ControllerState.Idle;
    
    private Vector2 GetPointerPosition() => _pointerPositionAction.ReadValue<Vector2>();
}