using DG.Tweening;
using UnityEngine;
using System;
using System.Collections.Generic;

public sealed class ScreenShakeController : MonoBehaviour
{
    private static ScreenShakeController _instance;
    public static ScreenShakeController Instance => _instance;
    
    [SerializeField] private AnimationCurve _shakeCurve;
    [SerializeField] private float _positionShakeStrengthModifier;
    [SerializeField] private float _rotationShakeStrengthModifier;
    [SerializeField] private Transform _cameraParent;
    
    private void Awake() => _instance = this;
    
    public void PlayScreenShake(ScreenShakeData screenShakeData, Vector3 position)
    {
        float focusValue = FocusMannager.Instance.GetFocusAtPosition(position);
        
        Vector3 intensityVector = new Vector3(1f, 1f, 0f) * screenShakeData.Intensity * focusValue;
        float duration = screenShakeData.Duration * focusValue;
        
        _cameraParent.DOShakePosition(duration, _positionShakeStrengthModifier * intensityVector).SetEase(_shakeCurve).SetLink(_cameraParent.gameObject).SetUpdate(true);
        _cameraParent.DOShakeRotation(duration, _rotationShakeStrengthModifier * intensityVector).SetEase(_shakeCurve).SetLink(_cameraParent.gameObject).SetUpdate(true);
    }
}

[Serializable] public sealed class ScreenShakeData
{
    public float Intensity = 0.6f;
    public float Duration = 0.3f;
}