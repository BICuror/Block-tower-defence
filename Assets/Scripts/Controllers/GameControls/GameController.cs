using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

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
            case ControllerState.Idle: TryStartItemInspection(); return; 
            case ControllerState.Dragging: _dragController.TryDragTo(GetPointerPosition()); break;
            case ControllerState.Rotating: _cameraRotationController.Rotate(GetPointerPosition()); break;
            case ControllerState.Inspecting: return;
        }
    }

    private void TryReturnToIdleState()
    {
        if (_currentControllerState == ControllerState.Inspecting)
        {
            if (_inspectorController.TryStopInspecting(GetPointerPosition()))
            {
                _currentControllerState = ControllerState.Idle;
            }
        }
    }

    private void TryPickUpDraggableOrRotateCamera()
    {
        StopInspecting();
        
        if (_dragController.PickedUpDraggable(GetPointerPosition()))
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
    
    private void TryStartItemInspection()
    {
        if (_inspectorController.TryToStartInspectingItem(GetPointerPosition())) 
        { 
            _currentControllerState = ControllerState.Inspecting;
        }
    }

    private void TryActivateOrStartInspecting(InputAction.CallbackContext context)
    {
        if (_currentControllerState == ControllerState.Dragging) return;
        
        StopInspecting();
        
        if (context.interaction is TapInteraction)
        {
            _dragController.ActivatedSomething(GetPointerPosition());
        }
        else if (context.interaction is HoldInteraction)
        {
            if (_inspectorController.TryToStartInspecting(GetPointerPosition()))
            {
                _currentControllerState = ControllerState.Inspecting;
            }
        }
    }

    private void StopInspecting()
    {
        _inspectorController.StopInspecting();
        _currentControllerState = ControllerState.Idle;
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

        _controls.TouchInput.LMB.started += _ => TryPickUpDraggableOrRotateCamera();

        _controls.TouchInput.LMB.canceled += _ => ReturnToIdleState();
        
        _controls.TouchInput.RMB.performed += TryActivateOrStartInspecting;

        _controls.TouchInput.PointerPosition.performed += _ => TryReturnToIdleState();

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