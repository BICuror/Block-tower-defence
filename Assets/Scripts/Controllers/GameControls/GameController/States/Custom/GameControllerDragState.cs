using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;
using GameControls.Controllers;
using UnityEngine;
using Zenject;

namespace GameControls.States
{
    public sealed class GameControllerDragState : GameControllerState
    {
        [Inject] private InspectorController _inspectorController;
        [Inject] private CursorController _cursorController;
        [Inject] private DragController _dragController;

        private InputAction _pointerPositionAction;
        private InputAction _activateAction;
        private InputAction _dragAction;

        protected override ControllerState State => ControllerState.Dragging;
        
        public override void Initialize()
        {
            _pointerPositionAction = InputActionMap["PointerPosition"];
            _activateAction = InputActionMap["ActivateOrDragCamera"];
            _dragAction = InputActionMap["DragOrRotateCamera"];
        }

        protected override void OnEnableState()
        {
            _activateAction.performed += TryActivateObject;
            _dragAction.canceled += InvokeTryExitState;
            _dragAction.started += TryEnterState;
        }

        protected override void OnDisableState()
        {
            _activateAction.performed -= TryActivateObject;
            _dragAction.canceled -= InvokeTryExitState;
            _dragAction.started -= TryEnterState;
        }

        private void TryActivateObject(InputAction.CallbackContext _)
        {
            _dragController.ActivatedSomething(GetPointerPosition());
        }

        private void TryEnterState(InputAction.CallbackContext _)
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

        public override bool CanEnterStateFrom(ControllerState currentState) => currentState is ControllerState.Idle
            or ControllerState.Inspecting or ControllerState.CameraRepositionDrag;

        public override bool CanExitStateTo(ControllerState currentState) => currentState is ControllerState.Idle;

        private Vector2 GetPointerPosition() => _pointerPositionAction.ReadValue<Vector2>();
    }
}