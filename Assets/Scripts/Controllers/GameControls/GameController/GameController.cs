using UnityEngine;

[RequireComponent(typeof(Camera))]

public sealed class GameController : MonoBehaviour
{
    [SerializeField] private TMPEffects.SerializedCollections.SerializedDictionary<ControllerState, GameControllerState> _states = new();
    [SerializeField] private CameraPositionController _cameraPositionController;
    [SerializeField] private CameraZoomController _cameraZoomController;
    [SerializeField] private HoverableController _hoverableController;
    [SerializeField] private float _modifiedTimeScale = 2f;
    
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
    
    private void ToggleTimeScale()
    {
        if (Time.timeScale == 1) Time.timeScale = _modifiedTimeScale;
        else Time.timeScale = 1;
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
        
        _controls.TouchInput.TimeToggle.performed += _ => ToggleTimeScale();

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