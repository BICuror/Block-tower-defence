using UnityEngine.InputSystem;
using GameControls.Features;
using Zenject;

namespace GameControls.Controllers
{
    public sealed class CameraKeyboardRepositionGameControllerFeature : GameControllerFeature
    {
        [Inject] private CameraController _cameraController;
        
        private InputAction _returnToDefaultCameraPosition;
        private InputAction _backwardInputAction;
        private InputAction _forwardInputAction;
        private InputAction _rightInputAction;
        private InputAction _leftInputAction;
        
        public override void Initialize()
        {
            _returnToDefaultCameraPosition = InputActionMap["ReturnDefaultCameraPosition"];
            _forwardInputAction = InputActionMap["CameraForward"];
            _backwardInputAction = InputActionMap["CameraBack"];
            _rightInputAction = InputActionMap["CameraRight"];
            _leftInputAction = InputActionMap["CameraLeft"];
        }

        protected override void OnEnableFeature()
        {            
            _rightInputAction.started += IncreaseXVelocity;
            _rightInputAction.canceled += DecreaseXVelocity;

            _leftInputAction.started += DecreaseXVelocity;
            _leftInputAction.canceled += IncreaseXVelocity;

            _forwardInputAction.started += IncreaseYVelocity;
            _forwardInputAction.canceled += DecreaseYVelocity;

            _backwardInputAction.started += DecreaseYVelocity;
            _backwardInputAction.canceled += IncreaseYVelocity;
            
            _returnToDefaultCameraPosition.performed += SetDefaultPosition;
        }

        protected override void OnDisableFeature()
        {
            _rightInputAction.started -= IncreaseXVelocity;
            _rightInputAction.canceled -= DecreaseXVelocity;

            _leftInputAction.started -= DecreaseXVelocity;
            _leftInputAction.canceled -= IncreaseXVelocity;

            _forwardInputAction.started -= IncreaseYVelocity;
            _forwardInputAction.canceled -= DecreaseYVelocity;

            _backwardInputAction.started -= DecreaseYVelocity;
            _backwardInputAction.canceled -= IncreaseYVelocity;
            
            _returnToDefaultCameraPosition.performed -= SetDefaultPosition;
        }
        
        private void SetDefaultPosition(InputAction.CallbackContext _) => _cameraController.SetDefaultPosition();
        
        private void IncreaseXVelocity(InputAction.CallbackContext _) => _cameraController.ChangeMovementInput(1, 0);
        private void DecreaseXVelocity(InputAction.CallbackContext _) => _cameraController.ChangeMovementInput(-1, 0);
        private void IncreaseYVelocity(InputAction.CallbackContext _) => _cameraController.ChangeMovementInput(0, 1);
        private void DecreaseYVelocity(InputAction.CallbackContext _) => _cameraController.ChangeMovementInput(0, -1);
    }
}