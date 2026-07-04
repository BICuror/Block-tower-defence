using CuroSettings;
using UnityEngine;
using Zenject;

namespace GameControls.Controllers
{
    public sealed class CameraRotationController : MonoBehaviour
    {
        [Inject] private CameraController _cameraController;

        [SerializeField] private Transform _cameraContainer;
        [SerializeField] private Transform _target;
        [SerializeField] private Camera _camera;

        [SerializeField] private float _distanceToTarget;
        [Range(0f, 85f)] [SerializeField] private float _maxYRotation;
        [Range(0f, 85f)] [SerializeField] private float _minYRotation;

        [SerializeField] private float _sensetivity;
        private FloatSetting _cameraRotationSensitivity;

        private Vector2 _screenResolution => new(Screen.width, Screen.height);
        
        private void Start()
        {
            _cameraRotationSensitivity = SettingsContainer.GetSetting<FloatSetting>(SettingsEnum.CameraRotationSensitivity);
            _cameraController.CameraPositionUpdated += UpdateCameraRotation;
            Rotate(Vector2.zero);
        }

        public void Rotate(Vector2 touchDelta)
        {
            UpdateCameraRotation(touchDelta);

            _cameraController.InvokeCameraRotatedEvent();
        }
        
        private void UpdateCameraRotation() => UpdateCameraRotation(Vector2.zero);
        private void UpdateCameraRotation(Vector2 touchDelta)
        {
            touchDelta /= _screenResolution;

            float rotationAroundYAxis = touchDelta.x * _sensetivity * _cameraRotationSensitivity.Value;
            float rotationAroundXAxis = -touchDelta.y * _sensetivity * _cameraRotationSensitivity.Value;

            float currentRotation = _cameraContainer.rotation.eulerAngles.x;

            _cameraContainer.position = _target.position;

            if (rotationAroundXAxis + currentRotation >= _maxYRotation) rotationAroundXAxis = _maxYRotation - currentRotation;
            else if (rotationAroundXAxis + currentRotation <= _minYRotation) rotationAroundXAxis = _minYRotation - currentRotation;

            _cameraContainer.Rotate(new Vector3(1, 0, 0), rotationAroundXAxis);

            _cameraContainer.Rotate(new Vector3(0, 1, 0), rotationAroundYAxis, Space.World);

            _cameraContainer.Translate(new Vector3(0, 0, -_distanceToTarget));
        }
    }
}