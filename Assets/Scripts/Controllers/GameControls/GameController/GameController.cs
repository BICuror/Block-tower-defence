using TMPEffects.SerializedCollections;
using UnityEngine.InputSystem;
using UnityEngine;
using Zenject;

public sealed class GameController : MonoBehaviour
{
    [Inject] private CameraPositionController _cameraPositionController;
    [Inject] private CameraZoomController _cameraZoomController;
    [Inject] private HoverableController _hoverableController; 
    [Inject] private TimeController _timeController;
    
    [SerializeField] private SerializedDictionary<ControllerState, GameControllerState> _states = new();
    [SerializeField] private PlayerInput _controls;

    private ControllerState _currentControllerState;
    private InputAction _pointerPositionAction;
    
    private void InitializeStates()
    {
        foreach (GameControllerState gameControllerState in _states.Values)
        {
            gameControllerState.Initialize(_controls.currentActionMap);
            gameControllerState.TriedToEnterState += TryEnterState;
            gameControllerState.TriedToExitState += TryExitState;
        }
    }

    private void TryEnterState(ControllerState state)
    {
        if (_states[_currentControllerState].CanExitStateTo(state) && _states[state].CanEnterStateFrom(_currentControllerState))
        {
            _states[_currentControllerState].Exit();   
            _currentControllerState = state;
            _states[state].Enter();   
        }
    }
    
    private void TryExitState(ControllerState state)
    {
        if (_currentControllerState != state) return;
        
        ControllerState idleState = ControllerState.Idle;
        
        if (_states[_currentControllerState].CanExitStateTo(idleState) && _states[idleState].CanEnterStateFrom(_currentControllerState))
        {
            _states[_currentControllerState].Exit();   
            _currentControllerState = idleState;
            _states[idleState].Enter();   
        }
    }
    
    private void FixedUpdate()
    {
        _hoverableController.CheckHover(_pointerPositionAction.ReadValue<Vector2>());
    }

    #region Enable\Disable

    public void Enable()
    {
        _controls.currentActionMap.Enable();
    }

    public void Disable()
    {
        _controls.currentActionMap.Disable();
    } 

    private void Start()
    {   
        CreateControls();

        Enable();
    }
    
    private void CreateControls()
    {
        _pointerPositionAction = _controls.currentActionMap["PointerPosition"];
        
        _controls.currentActionMap["ReturnDefaultCameraPosition"].performed += _ => _cameraPositionController.SetDefaultPosition();

        _controls.currentActionMap["ScrolledUp"].performed += _ => _cameraZoomController.ZoomIn();
        _controls.currentActionMap["ScrolledDown"].performed += _ => _cameraZoomController.ZoomOut();
        
        _controls.currentActionMap["ToggleTime"].performed += _ => _timeController.ToggleTimeScale();

        InitializeStates();
    }

    private void OnDestroy()
    {
        Disable();
        _controls = null;
    }

    #endregion
}