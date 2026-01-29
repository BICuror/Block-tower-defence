using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Camera))]

public sealed class CameraZoomController : MonoBehaviour
{
    [Header("ZoomSettings")]
    [SerializeField] private float _zoomSensetivity;
    [SerializeField] private float _minZoomValue;
    [SerializeField] private float _maxZoomValue;
    
    [Header("ZoomSmoothingSettings")]
    [SerializeField] private float _zoomSmoothingDuration = 0.25f;
    [SerializeField] private AnimationCurve _zoomSmoothingCurve;
    
    private Camera _camera;
    
    private float _finalZoom;
    private Tween _zoomTween;
    
    private void OnEnable()
    {
        _camera = GetComponent<Camera>();

        _finalZoom = _camera.orthographicSize;
    }

    public void ZoomIn() => ChangeZoomValue(-_zoomSensetivity);
    public void ZoomOut() => ChangeZoomValue(_zoomSensetivity);

    private void ChangeZoomValue(float changeValue)
    {
        if (InspectionTooltipManager.Instance.NonIdleTooltipsOpened) return;
        
        _finalZoom = Mathf.Clamp(_finalZoom + changeValue, _minZoomValue, _maxZoomValue);

        if (_zoomTween != null) _zoomTween.Kill();
        _zoomTween = DOVirtual.Float(_camera.orthographicSize, _finalZoom, _zoomSmoothingDuration, SetZoom).SetEase(_zoomSmoothingCurve);
    }

    private void SetZoom(float value) => _camera.orthographicSize = value;
}