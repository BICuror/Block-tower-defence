using System.Collections.Generic;
using GameControls.Controllers;
using GameControls.Features;
using UnityEngine;
using Zenject;

public sealed class FocusMannager : MonoBehaviour
{
    private static FocusMannager _instance;
    public static FocusMannager Instance => _instance;
    
    [Inject] private CameraZoomGameControllerFeature _cameraZoomController;
    [Inject] private CameraController _cameraController;

    [Header("Viewport")] 
    [SerializeField] private Camera _camera;
    [Range(0f, 1.5f)] [SerializeField] private float _maxViewportDistance;
    [Range(0f, 1f)] [SerializeField] private float _borderDecreaseMultiplier;
    [Range(0f, 1f)] [SerializeField] private float _outOfViewportReduction;
    
    private List<FocusElement> _focusElements = new();

    private void Awake() => _instance = this;
    
    private void Start()
    {
        _cameraZoomController.ZoomChanged += UpdateAllFocusElementValues;
        _cameraController.CameraRotated += UpdateAllFocusElementValues;
    }

    public void AddFocusElement(FocusElement focusElement)
    {
        _focusElements.Add(focusElement);
        
        UpdateFocusElementValue(focusElement);
    }

    public void RemoveFocusElement(FocusElement focusElement)
    {
        _focusElements.Remove(focusElement);
    }

    public void UpdateFocusElementValue(FocusElement focusElement)
    {
        Vector2 viewportPosition = _camera.WorldToViewportPoint(focusElement.transform.position);
        
        float resultValue = GetFocusAtViewportPosition(viewportPosition);
        
        focusElement.SetFocusValue(resultValue);
        focusElement.UpdateViewportPosition(viewportPosition);
    }

    public float GetFocusAtPosition(Vector3 position)
    {
        Vector2 viewportPosition = _camera.WorldToViewportPoint(position);
        
        return GetFocusAtViewportPosition(viewportPosition);
    }

    private float GetFocusAtViewportPosition(Vector2 viewportPosition)
    {
        float distance = Vector2.Distance(viewportPosition, new Vector2(0.5f, 0.5f));

        distance = Mathf.Min(distance, _maxViewportDistance) / _maxViewportDistance;

        float resultValue = 1;

        if (viewportPosition.x < 0 || viewportPosition.x > 1 || viewportPosition.y < 0 || viewportPosition.y > 1)
        {
            resultValue -= _outOfViewportReduction;
        }
        
        resultValue -= _borderDecreaseMultiplier * distance;
        
        return resultValue;
    }

    private void UpdateAllFocusElementValues()
    {
        _focusElements.ForEach(UpdateFocusElementValue);
    }
}