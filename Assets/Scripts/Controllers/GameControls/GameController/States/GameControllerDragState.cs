using UnityEngine.InputSystem.Interactions;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using System;

public sealed class GameControllerDragState : GameControllerState
{
    [SerializeField] private InspectorController _inspectorController;
    [SerializeField] private DragController _dragController;
    
    private GameControls _controls;
    
    protected override ControllerState State => ControllerState.Dragging;

    public override void Initialize(GameControls controls)
    {
        _controls = controls;
        
        _controls.TouchInput.LMB.started += _ => TryEnterState();
        _controls.TouchInput.LMB.canceled += _ => InvokeTryExitState();
        _controls.TouchInput.RMB.performed += TryActivateObject;
    }

    private void TryActivateObject(InputAction.CallbackContext context)
    {
        if (context.interaction is TapInteraction)
        {
            _dragController.ActivatedSomething(GetPointerPosition());
        }
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
    }

    private async UniTask DragObject()
    {
        while (_controls.TouchInput.LMB.IsPressed() && IsActive)
        {
            await UniTask.WaitForFixedUpdate();
            
            _dragController.TryDragTo(GetPointerPosition());
        }
        
        _dragController.DropDraggable(GetPointerPosition());
        InvokeTryExitState();
    }
    
    public override bool CanEnterStateFrom(ControllerState currentState) => currentState is ControllerState.Idle or ControllerState.Inspecting or ControllerState.PositionDragging;

    public override bool CanExitStateTo(ControllerState currentState) => currentState is ControllerState.Idle;
    
    private Vector2 GetPointerPosition() => _controls.TouchInput.PointerPosition.ReadValue<Vector2>();
}