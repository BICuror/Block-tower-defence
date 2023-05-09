using UnityEngine;

public sealed class Controller : MonoBehaviour
{
    private Controls _controls;

    [SerializeField] private DragController _dragController;
    [SerializeField] private CameraRotationController _cameraRotationController;
    [SerializeField] private CameraZoomController _cameraZoomController;

    public ControllerState _currentControllerState;
    public enum ControllerState 
    {
        Idle,
        Dragging,
        Rotating,
        Zooming
    }   

    private void Setup()
    {
        _controls.TouchInput.FirstTouchEvent.started += _ => TryPickUpDraggableOrRotateCamera();

        _controls.TouchInput.FirstTouchEvent.canceled += _ => ReturnToIdleState();

        _controls.TouchInput.SecondTouchEvent.started += _ => StartZooming();

        _controls.TouchInput.SecondTouchEvent.canceled += _ => StopZooming();
    }

    private void FixedUpdate()
    {
        switch(_currentControllerState)
        {
            case ControllerState.Idle: return;
            case ControllerState.Dragging: _dragController.TryDragTo(GetFirstTouchPosition()); break;
            case ControllerState.Rotating: _cameraRotationController.Rotate(GetFirstTouchPosition()); break;
            case ControllerState.Zooming: _cameraZoomController.Zooming(GetFirstTouchPosition(), GetSecondTouchPosition()); break;
        }
    }

    private void TryPickUpDraggableOrRotateCamera()
    {
        if (_dragController.PickedUpDraggable(GetFirstTouchPosition()))
        {
            _currentControllerState = ControllerState.Dragging;

            _dragController.PickUpDraggable(GetFirstTouchPosition());
        }
        else if (_dragController.ActivatedSomething(GetFirstTouchPosition()) == false)
        {
            _currentControllerState = ControllerState.Rotating;

            _cameraRotationController.SetPreviousMousePosition(GetFirstTouchPosition());
        }
    }

    private void ReturnToIdleState()
    {
        if (_currentControllerState == ControllerState.Dragging)
        {
            _dragController.DropDraggable();
        }

        _currentControllerState = ControllerState.Idle;
    }

    private void StartZooming()
    {
        _currentControllerState = ControllerState.Zooming;

        _cameraZoomController.StartZooming(GetFirstTouchPosition(), GetSecondTouchPosition());
    }

    private void StopZooming()
    {
        _currentControllerState = ControllerState.Rotating;
    }



    private Vector2 GetFirstTouchPosition()
    {
        return _controls.TouchInput.FirstTouch.ReadValue<Vector2>();
    }
    private Vector2 GetSecondTouchPosition()
    {
        return _controls.TouchInput.SecondTouch.ReadValue<Vector2>();
    }

    private void OnEnable() 
    {
        _controls = new Controls();

        _controls.Enable();

        Setup();
    } 

    private void OnDisable() 
    {
        _controls.Disable();

        _controls = null;
    }
}