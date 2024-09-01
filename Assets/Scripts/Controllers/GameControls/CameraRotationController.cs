using UnityEngine;
using UnityEngine.Events;
    
[RequireComponent(typeof(GameController))]

public sealed class CameraRotationController : MonoBehaviour
{       
    [SerializeField] private float _distanceToTarget;
    [Range(0f, 85f)] [SerializeField] private float _maxYRotation;
    [Range(0f, 85f)] [SerializeField] private float _minYRotation;

    [Range(1f, 1000f)] [SerializeField] private float _sensetivity;

    private Camera _camera;
    private Transform _target;

    private Vector3 _previousPosition;
    private Vector2 _previousTouchPosition = Vector2.zero;

    public UnityEvent CameraRotated;

    private void OnEnable() => _camera = GetComponent<Camera>();
    
    public void SetTarget(Transform newTarget) 
    {
        _target = newTarget; 
    
        Rotate(_previousTouchPosition);
    }

    public void SetPreviousMousePosition(Vector2 mousePosition) => _previousPosition = _camera.ScreenToViewportPoint(mousePosition);

    public void Rotate(Vector2 touchPosition)
    {       
        _previousTouchPosition = touchPosition;
        Vector3 newPosition = _camera.ScreenToViewportPoint(touchPosition);
        Vector3 direction = _previousPosition - newPosition;
        
        float rotationAroundYAxis = -direction.x * _sensetivity; 
        float rotationAroundXAxis = direction.y * _sensetivity; 

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
