using UnityEngine.Events;
using CuroSettings;
using UnityEngine;
using Zenject;

namespace GameControls.Controllers
{
    public sealed class CameraRotationController : MonoBehaviour
    {
        [Inject] private CameraController _cameraController;
        
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
            _cameraRotationSensitivity =
                SettingsContainer.GetSetting<FloatSetting>(SettingsEnum.CameraRotationSensitivity);
            UpdateCameraRotation();
        }

        public void UpdateCameraRotation() => Rotate(Vector2.zero);

        public void Rotate(Vector2 touchDelta)
        {
            touchDelta /= _screenResolution;

            float rotationAroundYAxis = touchDelta.x * _sensetivity * _cameraRotationSensitivity.Value;
            float rotationAroundXAxis = -touchDelta.y * _sensetivity * _cameraRotationSensitivity.Value;

            float currentRotation = transform.rotation.eulerAngles.x;

            transform.position = _target.position;

            if (rotationAroundXAxis + currentRotation >= _maxYRotation)
                rotationAroundXAxis = _maxYRotation - currentRotation;
            else if (rotationAroundXAxis + currentRotation <= _minYRotation)
                rotationAroundXAxis = _minYRotation - currentRotation;

            transform.Rotate(new Vector3(1, 0, 0), rotationAroundXAxis);

            transform.Rotate(new Vector3(0, 1, 0), rotationAroundYAxis, Space.World);

            transform.Translate(new Vector3(0, 0, -_distanceToTarget));

            _cameraController.InvokeCameraRotatedEvent();
        }

        private void InvokeCameraRotatedEvent() => _cameraController.InvokeCameraRotatedEvent();
    }
}