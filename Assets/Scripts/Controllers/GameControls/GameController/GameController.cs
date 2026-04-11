using UnityEngine;
using Zenject;

public sealed class GameController : MonoBehaviour
{
    [Inject] private CameraPositionController _cameraPositionController;
    [Inject] private CameraZoomController _cameraZoomController;
    [Inject] private HoverableController _hoverableController; 
    [Inject] private TimeController _timeController;
    
    [SerializeField] private TMPEffects.SerializedCollections.SerializedDictionary<ControllerState, GameControllerState> _states = new();
    
    private GameControls _controls;

    private ControllerState _currentControllerState;

    private void InitializeStates()
    {
        foreach (GameControllerState gameControllerState in _states.Values)
        {
            gameControllerState.Initialize(_controls);
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
        _hoverableController.CheckHover(_controls.TouchInput.PointerPosition.ReadValue<Vector2>());
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

        _controls.TouchInput.ReturnDefaultCameraPosition.performed += _ => _cameraPositionController.SetDefaultPosition();

        _controls.TouchInput.ScrolledUp.started += _ => _cameraZoomController.ZoomIn();
        _controls.TouchInput.ScrolledDown.started += _ => _cameraZoomController.ZoomOut();
        
        _controls.TouchInput.TimeToggle.performed += _ => _timeController.ToggleTimeScale();

        InitializeStates();
    }

    private void OnDestroy() 
    {
        Disable();
        _controls.Dispose();
        _controls = null;
    }

    #endregion
}