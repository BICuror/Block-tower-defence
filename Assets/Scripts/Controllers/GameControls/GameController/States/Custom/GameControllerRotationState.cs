using GameControls.Controllers;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;
using UnityEngine;
using Zenject;

namespace GameControls.States
{
    public sealed class GameControllerRotationState : GameControllerState
    {
        [Inject] private CameraRotationController _cameraRotationController;
        [Inject] private InspectorController _inspectorController;

        private InputAction _pointerDeltaAction;
        private InputAction _dragAction;

        protected override ControllerState State => ControllerState.Rotating;
        
        public override void Initialize()
        {
            _pointerDeltaAction = InputActionMap["PointerDelta"];
            _dragAction = InputActionMap["DragOrRotateCamera"];
        }

        protected override void OnEnableState()
        {
            _dragAction.canceled += InvokeTryExitState;
            _dragAction.started += TryEnterState;
        }

        protected override void OnDisableState()
        {
            _dragAction.canceled -= InvokeTryExitState;
            _dragAction.started -= TryEnterState;
        }
        
        public override bool CanEnterStateFrom(ControllerState currentState) => currentState is ControllerState.Idle
            or ControllerState.CameraRepositionDrag or ControllerState.Inspecting;

        public override bool CanExitStateTo(ControllerState currentState) => currentState is ControllerState.Inspecting
            or ControllerState.Idle or ControllerState.CameraRepositionDrag;

        private void TryEnterState(InputAction.CallbackContext _)
        {
            if (_inspectorController.IsHoveredOverNonIdleUI()) return;

            InvokeTryEnterState();
        }

        protected override void OnEnter()
        {
            RotateCamera().Forget();
        }

        private async UniTask RotateCamera()
        {
            while (_dragAction.IsPressed() && IsActive)
            {
                await UniTask.WaitForFixedUpdate();

                if (GetPointerDelta() != Vector2.zero) _cameraRotationController.Rotate(GetPointerDelta());
            }

            InvokeTryExitState();
        }
        
        private Vector2 GetPointerDelta() => _pointerDeltaAction.ReadValue<Vector2>() / Time.timeScale;
    }
}