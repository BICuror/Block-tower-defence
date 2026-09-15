using GameControls.Controllers;
using Cysharp.Threading.Tasks;
using GameControls.Features;
using UnityEngine.InputSystem;
using UnityEngine;
using Zenject;

namespace GameControls.States
{
    public sealed class GameCameraPositionDragState : GameControllerState
    {
        [Inject] private CameraZoomGameControllerFeature _cameraZoomGameController;
        [Inject] private InspectorController _inspectorController;
        [Inject] private CursorController _cursorController;
        [Inject] private CameraController _cameraController;
        [Inject] private DragController _dragController;

        private InputAction _pointerPositionAction;
        private InputAction _pointerDeltaAction;
        private InputAction _cameraDragAction;

        protected override ControllerState State => ControllerState.CameraRepositionDrag;
        
        public override void Initialize()
        {
            _pointerPositionAction = InputActionMap["PointerPosition"];
            _pointerDeltaAction = InputActionMap["PointerDelta"];
            _cameraDragAction = InputActionMap["ActivateOrDragCamera"];
        }

        protected override void OnEnableState()
        {
            _cameraDragAction.canceled += InvokeTryExitState;
            _cameraDragAction.started += TryEnterState;
        }

        protected override void OnDisableState()
        {
            _cameraDragAction.canceled -= InvokeTryExitState;
            _cameraDragAction.started -= TryEnterState;
        }
        
        public override bool CanEnterStateFrom(ControllerState currentState) =>
            currentState is ControllerState.Idle or ControllerState.Rotating or ControllerState.Inspecting;

        public override bool CanExitStateTo(ControllerState currentState) =>
            currentState is ControllerState.Idle or ControllerState.Rotating or ControllerState.Dragging;


        private void TryEnterState(InputAction.CallbackContext _)
        {
            if (_inspectorController.HoveredOverInspectable(GetPointerPosition())) return;

            if (_dragController.HoveredOverActivatable(GetPointerPosition())) return;

            if (_inspectorController.IsHoveredOverNonIdleUI()) return;

            InvokeTryEnterState();
        }

        protected override void OnEnter()
        {
            RepositionCamera().Forget();

            _cursorController.SetCursorState(CursorController.CursorState.Move);
        }

        private async UniTask RepositionCamera()
        {
            while (_cameraDragAction.IsPressed() && IsActive)
            {
                await UniTask.WaitForFixedUpdate();

                _cameraController.DragCamera(GetPointerDelta() * _cameraZoomGameController.CurrentZoom);
            }

            InvokeTryExitState();
        }
        
        private Vector2 GetPointerPosition() => _pointerPositionAction.ReadValue<Vector2>();
        private Vector2 GetPointerDelta() => _pointerDeltaAction.ReadValue<Vector2>() / Time.timeScale;
    }
}