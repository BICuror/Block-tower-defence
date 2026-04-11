using UnityEngine.Events;
using CuroSettings;
using UnityEngine;
    
[RequireComponent(typeof(GameController))]

public sealed class CameraRotationController : MonoBehaviour
{       
    [SerializeField] private Transform _target;
    [SerializeField] private Camera _camera;
    
    [SerializeField] private float _distanceToTarget;
    [Range(0f, 85f)] [SerializeField] private float _maxYRotation;
    [Range(0f, 85f)] [SerializeField] private float _minYRotation;

    [Range(1f, 1000f)] [SerializeField] private float _sensetivity;
    private FloatSetting _cameraRotationSensitivity;

    private Vector2 _previousTouchPosition = Vector2.zero;
    private Vector3 _previousPosition;

    public UnityEvent CameraRotated;

    private void Start()
    {
        _cameraRotationSensitivity = SettingsContainer.GetSetting<FloatSetting>(SettingsEnum.CameraRotationSensitivity);
        UpdateCameraRotation();
    }

    public void UpdateCameraRotation() => Rotate(_previousTouchPosition);

    public void SetPreviousMousePosition(Vector2 mousePosition) => _previousPosition = _camera.ScreenToViewportPoint(mousePosition);

    public void Rotate(Vector2 touchPosition)
    {       
        _previousTouchPosition = touchPosition;
        Vector3 newPosition = _camera.ScreenToViewportPoint(touchPosition);
        Vector3 direction = _previousPosition - newPosition;
        
        float rotationAroundYAxis = -direction.x * _sensetivity * _cameraRotationSensitivity.Value; 
        float rotationAroundXAxis = direction.y * _sensetivity * _cameraRotationSensitivity.Value; 

        float currentRotation = transform.rotation.eulerAngles.x;

        transform.position = _target.position;
            
        if (rotationAroundXAxis + currentRotation >= _maxYRotation) rotationAroundXAxis = _maxYRotation - currentRotation;
        else if (rotationAroundXAxis + currentRotation <= _minYRotation) rotationAroundXAxis = _minYRotation - currentRotation;  
        
        transform.Rotate(new Vector3(1, 0, 0), rotationAroundXAxis);
        
        transform.Rotate(new Vector3(0, 1, 0), rotationAroundYAxis, Space.World); 

        transform.Translate(new Vector3(0, 0, -_distanceToTarget));

        _previousPosition = newPosition;

        InvokeCameraRotatedEvent();
    }

    public void InvokeCameraRotatedEvent() => CameraRotated.Invoke();
}
