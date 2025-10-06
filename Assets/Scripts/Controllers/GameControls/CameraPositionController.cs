using UnityEngine;
using Zenject;
using System;

public sealed class CameraPositionController : MonoBehaviour
{
    [Inject] private IslandDataContainer _islandDataContainer;
    [SerializeField] private Transform _cameraCenter;
    [SerializeField] private float _cameraCenterMovementSpeed = 0.3f;
    [SerializeField] private float _height = 5f;
    [Range(0f, 1f)] [SerializeField] private float _easingSleepThreshold = 0.1f;
    [Range(0f, 1f)] [SerializeField] private float _cameraEasingSpeed = 0.9f;
    [Range(0f, 1f)] [SerializeField] private float _cameraRadiusScale = 0.5f;
    
    private float _islandRadius;
    private CameraPositionControls _controls;
    private Vector2 _movementInput;
    private Vector2 _currentPosition;
    private Camera _camera;

    public event Action CameraPositionUpdated;
    
    private void Awake()
    {
        _islandRadius = (_islandDataContainer.Data.IslandSize - 1) / 2f;
        
        _camera = Camera.main;
        
        SetDefaultPosition();
        _cameraCenter.position = new Vector3(_currentPosition.x, _height, _currentPosition.y);
    }

    public void SetDefaultPosition()
    {
        _currentPosition = new Vector2(_islandRadius, _islandRadius);
    }
    
    private void FixedUpdate()
    {
        if (ControllerIsIdle()) return;
        
        CameraPositionUpdated?.Invoke();
        
        _cameraCenter.position = Vector3.Lerp(_cameraCenter.position, new Vector3(_currentPosition.x, _height, _currentPosition.y), _cameraEasingSpeed);
             
        Vector2 cameraForward = new Vector2(_camera.transform.forward.x, _camera.transform.forward.z);
        Vector2 cameraRight = new Vector2(_camera.transform.right.x, _camera.transform.right.z);

        Vector2 movementDirection = (cameraForward * _movementInput.y) + (cameraRight * _movementInput.x);

        _currentPosition += movementDirection.normalized * _cameraCenterMovementSpeed;

        float higherBorder = _islandRadius + _islandRadius * _cameraRadiusScale;
        float lowerBorder = _islandRadius - _islandRadius * _cameraRadiusScale;
        
        if (_currentPosition.x > higherBorder) _currentPosition.x = higherBorder;
        else if (_currentPosition.x < lowerBorder) _currentPosition.x = lowerBorder;
        
        if (_currentPosition.y > higherBorder) _currentPosition.y = higherBorder;
        else if (_currentPosition.y < lowerBorder) _currentPosition.y = lowerBorder;
    }

    private bool ControllerIsIdle() => _movementInput == Vector2.zero && Vector2.Distance(new Vector2(_cameraCenter.position.x, _cameraCenter.position.z), _currentPosition) < _easingSleepThreshold;
        
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