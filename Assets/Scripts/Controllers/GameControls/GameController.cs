using UnityEngine;

[RequireComponent(typeof(Camera))]

public sealed class GameController : MonoBehaviour
{
    [SerializeField] private DragController _dragController;
    [SerializeField] private CameraRotationController _cameraRotationController;
    [SerializeField] private CameraZoomController _cameraZoomController;
    [SerializeField] private InspectorController _inspectorController;

    private GameControls _controls;

    private ControllerState _currentControllerState;

    public ControllerState State => _currentControllerState;

    private void FixedUpdate()
    {
        switch(_currentControllerState)
        {
            case ControllerState.Idle: return; 
            case ControllerState.Dragging: _dragController.TryDragTo(GetPointerPosition()); break;
            case ControllerState.Rotating: _cameraRotationController.Rotate(GetPointerPosition()); break;
            case ControllerState.Inspecting: return;
        }
    }

    private void UpdateInspectionState()
    {
        if (_currentControllerState == ControllerState.Idle)
        {
            if (_inspectorController.TryToStartInspecting(GetPointerPosition()))
            {
                _currentControllerState = ControllerState.Inspecting;
            }
        }
        else if (_currentControllerState == ControllerState.Inspecting)
        {
            if (_inspectorController.TryStopInspecting(GetPointerPosition()))
            {
                _currentControllerState = ControllerState.Idle;
            }
        }
    }

    private void TryPickUpDraggableOrRotateCamera()
    {
        _inspectorController.StopInspecting();

        if(_dragController.ActivatedSomething(GetPointerPosition()))
        {
            _currentControllerState = ControllerState.Idle;
        }
        else if (_dragController.PickedUpDraggable(GetPointerPosition()))
        {
            _currentControllerState = ControllerState.Dragging;

            _dragController.PickUpDraggable(GetPointerPosition());
        }
        else
        {
            _cameraRotationController.SetPreviousMousePosition(GetPointerPosition());
            
            _currentControllerState = ControllerState.Rotating;   
        }
    }

    private void ReturnToIdleState()
    {
        if (_currentControllerState == ControllerState.Dragging)
        {
            _dragController.DropDraggable(GetPointerPosition());
        }

        _currentControllerState = ControllerState.Idle;
    }

    private Vector2 GetPointerPosition()
    {
        return _controls.TouchInput.PointerPosition.ReadValue<Vector2>();
    }

    #region Enable\Disable

    public void Enable() => _controls.Enable();
    public void Disable() => _controls.Disable();

    private void Start()
    {   
        CreateControls();

        Enable();
    }
    
    private void CreateControls()
    {
        _controls = new GameControls();

        _controls.TouchInput.PointerClick.started += _ => TryPickUpDraggableOrRotateCamera();

        _controls.TouchInput.PointerClick.canceled += _ => ReturnToIdleState();

        _controls.TouchInput.PointerPosition.performed += _ => UpdateInspectionState();

        _controls.TouchInput.ScrolledUp.started += _ => _cameraZoomController.ZoomIn();
        _controls.TouchInput.ScrolledDown.started += _ => _cameraZoomController.ZoomOut();
    }

    private void OnDestroy() 
    {
        Disable();

        _controls = null;
    }

    #endregion 
    
    public enum ControllerState 
    {
        Idle,
        Dragging,
        Rotating,
        Inspecting,
    }   
}