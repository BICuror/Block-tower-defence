using UnityEngine.InputSystem.Interactions;
using UnityEngine.InputSystem;
using UnityEngine;

[RequireComponent(typeof(Camera))]

public sealed class GameController : MonoBehaviour
{
    [SerializeField] private CameraRotationController _cameraRotationController;
    [SerializeField] private CameraPositionController _cameraPositionController;
    [SerializeField] private CameraZoomController _cameraZoomController;
    [SerializeField] private InspectorController _inspectorController;
    [SerializeField] private DragController _dragController;

    private GameControls _controls;

    private ControllerState _currentControllerState;

    public ControllerState State => _currentControllerState;

    private void Awake()
    {
        _cameraPositionController.CameraPositionUpdated += () => _cameraRotationController.UpdateCameraRotation();
    }

    private void FixedUpdate()
    {
        switch(_currentControllerState)
        {
            case ControllerState.Idle: TryToInspect(); return; 
            case ControllerState.Dragging: _dragController.TryDragTo(GetPointerPosition()); break;
            case ControllerState.Rotating: _cameraRotationController.Rotate(GetPointerPosition()); break;
        }
    }

    private void TryPickUpOrRotateCamera()
    {
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
    
    private void TryActivateOrStartInspecting(InputAction.CallbackContext context)
    {
        if (_currentControllerState == ControllerState.Dragging) return;
        
        if (context.interaction is TapInteraction)
        {
            _dragController.ActivatedSomething(GetPointerPosition());
        }
        else if (context.interaction is HoldInteraction)
        {
            _inspectorController.TryToStartInspecting(GetPointerPosition());
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

    private void TryToInspect() => _inspectorController.TryToStartIdleInspecting(GetPointerPosition());
    
    private Vector2 GetPointerPosition()
    {
        return _controls.TouchInput.PointerPosition.ReadValue<Vector2>();
    }

    #region Enable\Disable

    public void Enable()
    {
        _controls.Enable();
        _cameraPositionController.Enable();
    }

    public void Disable()
    {
        _controls.Disable();
        _cameraPositionController.Disable();
    } 

    private void Start()
    {   
        CreateControls();

        Enable();
    }
    
    private void CreateControls()
    {
        _controls = new GameControls();

        _controls.TouchInput.LMB.started += _ => TryPickUpOrRotateCamera();

        _controls.TouchInput.LMB.canceled += _ => ReturnToIdleState();
        
        _controls.TouchInput.RMB.performed += TryActivateOrStartInspecting;
        
        _controls.TouchInput.ReturnDefaultCameraPosition.performed += _ => _cameraPositionController.SetDefaultPosition();

        _controls.TouchInput.ScrolledUp.started += _ => _cameraZoomController.ZoomIn();
        _controls.TouchInput.ScrolledDown.started += _ => _cameraZoomController.ZoomOut();
    }

    private void OnDestroy() 
    {
        Disable();
        _controls.Dispose();
        _controls = null;
    }

    #endregion 
    
    public enum ControllerState 
    {
        Idle,
        Dragging,
        Rotating,
    }   
}