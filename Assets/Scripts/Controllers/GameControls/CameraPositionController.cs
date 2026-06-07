using UnityEngine.InputSystem;
using CuroSettings;
using UnityEngine;
using Zenject;
using System;

public sealed class CameraPositionController : MonoBehaviour
{
    [Inject] private IslandDataContainer _islandDataContainer;
    
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private Transform _cameraCenter;
    [SerializeField] private Camera _camera;
    
    [Header("MovementSettings")]
    [SerializeField] private float _cameraCenterMovementSpeed = 0.3f;
    [SerializeField] private float _cameraDragSpeed = 0.003f;
    [SerializeField] private float _height = 5f;
    [Range(0f, 1f)] [SerializeField] private float _easingSleepThreshold = 0.1f;
    [Range(0f, 1f)] [SerializeField] private float _cameraEasingSpeed = 0.9f;
    [Range(0f, 1f)] [SerializeField] private float _cameraRadiusScale = 0.5f;
    
    private FloatSetting _cameraMovementSensitivity;
    private FloatSetting _cameraDragSensitivity;
    
    private Vector2 _currentPosition;
    private Vector2 _movementInput;
    private float _highestBorder;
    private float _lowestBorder;
    private float _islandRadius;
    
    private Vector2 _screenResolution => new(Screen.width, Screen.height);
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
        
        SetDefaultPosition();
        _cameraCenter.position = new Vector3(_currentPosition.x, _height, _currentPosition.y);
    }

    public void SetDefaultPosition()
    {
        _currentPosition = new Vector2(_islandDataContainer.Data.CenterPositionIndex, _islandDataContainer.Data.CenterPositionIndex);
    }
    
    public void DragCamera(Vector2 cameraDelta)
    {
        cameraDelta /= _screenResolution;
        Vector2 movementDirection = _cameraForward * -cameraDelta.y + _cameraRight * -cameraDelta.x;
        
        _currentPosition += movementDirection * (_cameraDragSpeed * _cameraDragSensitivity.Value);
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
    
    public void BindControls()
    {
        _playerInput.currentActionMap["CameraRight"].started += IncreaseXVelocity;
        _playerInput.currentActionMap["CameraRight"].canceled += DecreaseXVelocity;
        
        _playerInput.currentActionMap["CameraLeft"].started += DecreaseXVelocity;
        _playerInput.currentActionMap["CameraLeft"].canceled += IncreaseXVelocity;
        
        _playerInput.currentActionMap["CameraForward"].started += IncreaseYVelocity;
        _playerInput.currentActionMap["CameraForward"].canceled += DecreaseYVelocity;
        
        _playerInput.currentActionMap["CameraBack"].started += DecreaseYVelocity;
        _playerInput.currentActionMap["CameraBack"].canceled += IncreaseYVelocity;
    }

    public void UnbindControls()
    {
        _playerInput.currentActionMap["CameraRight"].started -= IncreaseXVelocity;
        _playerInput.currentActionMap["CameraRight"].canceled -= DecreaseXVelocity;
        
        _playerInput.currentActionMap["CameraLeft"].started -= DecreaseXVelocity;
        _playerInput.currentActionMap["CameraLeft"].canceled -= IncreaseXVelocity;
        
        _playerInput.currentActionMap["CameraForward"].started -= IncreaseYVelocity;
        _playerInput.currentActionMap["CameraForward"].canceled -= DecreaseYVelocity;
        
        _playerInput.currentActionMap["CameraBack"].started -= DecreaseYVelocity;
        _playerInput.currentActionMap["CameraBack"].canceled -= IncreaseYVelocity;
    }
    
    private void IncreaseXVelocity(InputAction.CallbackContext _) => _movementInput.x += 1;
    private void DecreaseXVelocity(InputAction.CallbackContext _) => _movementInput.x -= 1;
    private void IncreaseYVelocity(InputAction.CallbackContext _) => _movementInput.y += 1;
    private void DecreaseYVelocity(InputAction.CallbackContext _) => _movementInput.y -= 1;

    #endregion
}