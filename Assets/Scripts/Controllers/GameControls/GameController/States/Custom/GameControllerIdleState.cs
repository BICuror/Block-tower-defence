using GameControls.Controllers;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;
using UnityEngine;
using Zenject;

namespace GameControls.States
{
    public sealed class GameControllerIdleState : GameControllerState
    {
        [Inject] private InspectorController _inspectorController;
        [Inject] private CursorController _cursorController;

        private InputAction _pointerPositionAction;

        protected override ControllerState State => ControllerState.Idle;
        
        public override void Initialize()
        {
            _pointerPositionAction = InputActionMap["PointerPosition"];
        }

        protected override void OnEnableState() {}
        protected override void OnDisableState() {}
        
        public override bool CanEnterStateFrom(ControllerState currentState) => true;
        public override bool CanExitStateTo(ControllerState currentState) => true;

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

                if (hoveredOverInspectable &&
                    _cursorController.State != CursorController.CursorState.InspectionAvailable)
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

        private Vector2 GetPointerPosition() => _pointerPositionAction.ReadValue<Vector2>();
    }
}