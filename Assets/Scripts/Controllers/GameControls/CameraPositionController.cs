using CuroSettings;
using UnityEngine;
using Zenject;
using System;

public sealed class CameraPositionController : MonoBehaviour
{
    [Inject] private IslandDataContainer _islandDataContainer;
    [SerializeField] private Transform _cameraCenter;
    [SerializeField] private float _cameraCenterMovementSpeed = 0.3f;
    [SerializeField] private float _cameraDragSpeed = 0.003f;
    [SerializeField] private float _height = 5f;
    [Range(0f, 1f)] [SerializeField] private float _easingSleepThreshold = 0.1f;
    [Range(0f, 1f)] [SerializeField] private float _cameraEasingSpeed = 0.9f;
    [Range(0f, 1f)] [SerializeField] private float _cameraRadiusScale = 0.5f;
    
    private FloatSetting _cameraMovementSensitivity;
    private FloatSetting _cameraDragSensitivity;
    
    private CameraPositionControls _controls;
    private Vector2 _currentPosition;
    private Vector2 _movementInput;
    private float _highestBorder;
    private float _lowestBorder;
    private float _islandRadius;
    private Camera _camera;
    
    private Vector2 _previousCursorPosition;
    
    private Vector2 _screenResolution => new Vector2(Screen.width, Screen.height);
    private Vector2 _cameraForward => new Vector2(_camera.transform.forward.x, _camera.transform.forward.z).normalized;
    private Vector2 _cameraRight => new Vector2(_camera.transform.right.x, _camera.transform.right.z).normalized;

    public event Action CameraPositionUpdated;
    
    private void Awake()
    {
        _cameraMovementSensitivity = SettingsContainer.GetSetting<FloatSetting>(SettingsEnum.CameraMovementSensitivity);
        _cameraDragSensitivity = SettingsContainer.GetSetting<FloatSetting>(SettingsEnum.CameraDragSensitivity);
        
        _islandRadius = _islandDataContainer.Data.IslandRadius;
        _camera = Camera.main;
        
        _highestBorder = _islandRadius + _islandRadius * _cameraRadiusScale;
        _lowestBorder = _islandRadius - _islandRadius * _cameraRadiusScale;
       
        CreateControls();
        
        SetDefaultPosition();
        _cameraCenter.position = new Vector3(_currentPosition.x, _height, _currentPosition.y);
    }

    public void SetDefaultPosition()
    {
        _currentPosition = new Vector2(_islandDataContainer.Data.CenterPositionIndex, _islandDataContainer.Data.CenterPositionIndex);
    }
    
    public void CaptureCameraPosition(Vector2 cursorPosition) => _previousCursorPosition = cursorPosition;
    
    public void DragCamera(Vector2 cursorPosition)
    {
        Vector2 cursorPositionDifference = _previousCursorPosition - cursorPosition;

        cursorPositionDifference /= _screenResolution;
        
        Vector2 movementDirection = _cameraForward * cursorPositionDifference.y + _cameraRight * cursorPositionDifference.x;
        
        _currentPosition += movementDirection * (_cameraDragSpeed * _cameraDragSensitivity.Value);
        CaptureCameraPosition(cursorPosition);
        ClampCurrentPosition();
    }
    
    private void FixedUpdate()
    {
        if (ControllerIsIdle()) return;
        
        CameraPositionUpdated?.Invoke();
        
        _cameraCenter.position = Vector3.Lerp(_cameraCenter.position, new Vector3(_currentPosition.x, _height, _currentPosition.y), _cameraEasingSpeed / Time.timeScale);

        Vector2 movementDirection = _cameraForward * _movementInput.y + _cameraRight * _movementInput.x;

        _currentPosition += movementDirection.normalized * (_cameraCenterMovementSpeed * _cameraMovementSensitivity.Value / Time.timeScale);

        ClampCurrentPosition();
    }

    private void ClampCurrentPosition()
    {
        if (_currentPosition.x > _highestBorder) _currentPosition.x = _highestBorder;
        else if (_currentPosition.x < _lowestBorder) _currentPosition.x = _lowestBorder;
        
        if (_currentPosition.y > _highestBorder) _currentPosition.y = _highestBorder;
        else if (_currentPosition.y < _lowestBorder) _currentPosition.y = _lowestBorder;
    }

    private bool ControllerIsIdle() => _movementInput == Vector2.zero && Vector2.Distance(new Vector2(_cameraCenter.position.x, _cameraCenter.position.z), _currentPosition) < _easingSleepThreshold;
        
    #region Enable\Disable
    
    public void Enable() => _controls.Enable();
    public void Disable() => _controls.Disable();
    
    private void CreateControls()
    {
        _controls = new CameraPositionControls();

        _controls.Main.Right.started += _ => _movementInput.x += 1;
        _controls.Main.Right.canceled += _ => _movementInput.x -= 1;
        
        _controls.Main.Left.started += _ => _movementInput.x -= 1;
        _controls.Main.Left.canceled += _ => _movementInput.x += 1;
        
        _controls.Main.Forward.started += _ => _movementInput.y += 1;
        _controls.Main.Forward.canceled += _ => _movementInput.y -= 1;
        
        _controls.Main.Back.started += _ => _movementInput.y -= 1;
        _controls.Main.Back.canceled += _ => _movementInput.y += 1;
    }
    
    private void OnDestroy() 
    {
        Disable();
        _controls.Dispose();
        _controls = null;
    }
    
    #endregion 
}