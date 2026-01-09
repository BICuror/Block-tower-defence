using UnityEngine.InputSystem.Interactions;
using UnityEngine.InputSystem;
using UnityEngine;

[RequireComponent(typeof(Camera))]

public sealed class GameController : MonoBehaviour
{
    [SerializeField] private float _modifiedTimeScale = 2f;
    [SerializeField] private CameraRotationController _cameraRotationController;
    [SerializeField] private CameraPositionController _cameraPositionController;
    [SerializeField] private CameraZoomController _cameraZoomController;
    [SerializeField] private InspectorController _inspectorController;
    [SerializeField] private HoverableController _hoverableController;
    [SerializeField] private DragController _dragController;

    private GameControls _controls;

    private ControllerState _currentControllerState;

    public ControllerState State => _currentControllerState;

    private void Awake()
    {
        _cameraPositionController.CameraPositionUpdated += () => _cameraRotationController.UpdateCameraRotation();
        _inspectorController.InspectionStopped += () =>
        {
            if (_currentControllerState != ControllerState.Dragging) _currentControllerState = ControllerState.Idle;
        };
    }

    #region StateManagement

    private void FixedUpdate()
    {
        _hoverableController.CheckHover(GetPointerPosition()); 
        
        switch(_currentControllerState)
        {
            case ControllerState.Idle: TryIdleToInspect(); break;
            case ControllerState.Dragging: _dragController.TryDragTo(GetPointerPosition()); break;
            case ControllerState.Rotating: _cameraRotationController.Rotate(GetPointerPosition()); break;
            case ControllerState.Inspecting: break;
        }
    }
    
    private void TryIdleToInspect()
    {
        if (_inspectorController.TryToStartIdleInspecting(GetPointerPosition())) _currentControllerState = ControllerState.Inspecting;
    } 

    #endregion
    
    private void TryPickUpOrRotateCamera()
    {
        if (_dragController.PickedUpDraggable(GetPointerPosition(), out GameObject draggedObject))
        {
            if (_currentControllerState == ControllerState.Inspecting)
            {
                if (!_inspectorController.IsPossibleToDragInspectedItem(draggedObject)) return;
                
                _inspectorController.StopInspecting();
            }
            
            _currentControllerState = ControllerState.Dragging;

            _dragController.PickUpDraggable(GetPointerPosition());
        }
        else
        {
            if (_currentControllerState == ControllerState.Inspecting) return;
            
            _cameraRotationController.SetPreviousMousePosition(GetPointerPosition());
            
            _currentControllerState = ControllerState.Rotating;   
        }
    }
    
    private void TryActivateOrStartInspecting(InputAction.CallbackContext context)
    {
        if (_currentControllerState == ControllerState.Dragging) return;
        
        if (context.interaction is TapInteraction)
        {
            if (_currentControllerState == ControllerState.Inspecting) _inspectorController.StopInspecting();
            
            _dragController.ActivatedSomething(GetPointerPosition());
        }
        else if (context.interaction is HoldInteraction)
        {
            if (_inspectorController.TryToStartInspecting(GetPointerPosition())) _currentControllerState = ControllerState.Inspecting;
            else _currentControllerState = ControllerState.Idle;
        }
    }

    private void ReturnFromDragToIdleState()
    {
        if (_currentControllerState == ControllerState.Inspecting) return;
        if (_currentControllerState == ControllerState.Dragging) _dragController.DropDraggable(GetPointerPosition());
        
        _currentControllerState = ControllerState.Idle;
    }

    private void ToggleTimeScale()
    {
        if (Time.timeScale == 1) Time.timeScale = _modifiedTimeScale;
        else Time.timeScale = 1;
    }
    
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

        _controls.TouchInput.LMB.canceled += _ => ReturnFromDragToIdleState();
        
        _controls.TouchInput.RMB.performed += TryActivateOrStartInspecting;
        
        _controls.TouchInput.ReturnDefaultCameraPosition.performed += _ => _cameraPositionController.SetDefaultPosition();

        _controls.TouchInput.ScrolledUp.started += _ => _cameraZoomController.ZoomIn();
        _controls.TouchInput.ScrolledDown.started += _ => _cameraZoomController.ZoomOut();
        
        _controls.TouchInput.TimeToggle.performed += _ => ToggleTimeScale();
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
        Inspecting,
    }   
}