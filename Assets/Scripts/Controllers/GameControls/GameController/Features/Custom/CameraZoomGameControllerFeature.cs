using UnityEngine.InputSystem;
using CuroSettings;
using DG.Tweening;
using UnityEngine;
using System;

namespace GameControls.Features
{
    public sealed class CameraZoomGameControllerFeature : GameControllerFeature
    {
        [SerializeField] private Camera _camera;

        [Header("ZoomSettings")] 
        [SerializeField] private float _zoomSensetivity;

        [SerializeField] private float _minZoomValue;
        [SerializeField] private float _maxZoomValue;

        [Header("ZoomSmoothingSettings")] 
        [SerializeField] private float _zoomSmoothingDuration = 0.25f;

        [SerializeField] private AnimationCurve _zoomSmoothingCurve;
        private FloatSetting _zoomSensitivitySetting;

        private InputAction _zoomOutInputAction;
        private InputAction _zoomInInputAction;
        
        private float _finalZoom;
        private Tween _zoomTween;
        
        public event Action ZoomChanged;
        
        public override void Initialize()
        {
            _zoomSensitivitySetting = SettingsContainer.GetSetting<FloatSetting>(SettingsEnum.CameraZoomSensetiviy);

            _zoomOutInputAction = InputActionMap["ScrolledDown"];
            _zoomInInputAction = InputActionMap["ScrolledUp"];
            
            _finalZoom = _camera.orthographicSize;
        }

        protected override void OnEnableFeature()
        {
            _zoomOutInputAction.performed += ZoomOut;
            _zoomInInputAction.performed += ZoomIn;
        }

        protected override void OnDisableFeature()
        {
            _zoomOutInputAction.performed -= ZoomOut;
            _zoomInInputAction.performed -= ZoomIn;
        }

        private void ZoomOut(InputAction.CallbackContext _) => ChangeZoomValue(_zoomSensetivity * _zoomSensitivitySetting.Value);
        private void ZoomIn(InputAction.CallbackContext _) => ChangeZoomValue(-_zoomSensetivity * _zoomSensitivitySetting.Value);

        private void ChangeZoomValue(float changeValue)
        {
            if (InspectionTooltipManager.Instance.NonIdleTooltipsOpened) return;

            _finalZoom = Mathf.Clamp(_finalZoom + changeValue, _minZoomValue, _maxZoomValue);

            if (_finalZoom <= _minZoomValue || _finalZoom >= _maxZoomValue) return;

            if (_zoomTween != null) _zoomTween.Kill();
            _zoomTween = DOVirtual.Float(_camera.orthographicSize, _finalZoom, _zoomSmoothingDuration, SetZoom)
                .SetEase(_zoomSmoothingCurve).SetUpdate(true);
        }

        private void SetZoom(float value)
        {
            _camera.orthographicSize = value;

            ZoomChanged?.Invoke();
        }
    }
}