using UnityEngine;

[RequireComponent(typeof(Camera))]

public sealed class CameraZoomController : MonoBehaviour
{
    [SerializeField] private float _minZoomValue;
    [SerializeField] private float _maxZoomValue;
    private Camera _camera;
    
    private float _startZoom;
    
    private void OnEnable() => _camera = GetComponent<Camera>();

    public void StartZooming(Vector2 firstTouchPosition, Vector2 secondTouchPosition)
    {
        _startZoom = Vector2.Distance(firstTouchPosition, secondTouchPosition) + _camera.orthographicSize;
    }
    public void Zooming(Vector2 firstTouchPosition, Vector2 secondTouchPosition)
    {
        float zoomValue = _startZoom - Vector2.Distance(firstTouchPosition, secondTouchPosition);
        
        zoomValue = Mathf.Clamp(_minZoomValue, _maxZoomValue, zoomValue);

        _camera.orthographicSize = zoomValue;
    }
}
