using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using System;

public class Shaker : MonoBehaviour
{
    [Header("ShakeSettings")] 
    [SerializeField] private List<ShakeData> _shakeDatas;

    [Header("Links")]
    [SerializeField] protected Transform _mesh;
    private Vector3 _defaultScale = -Vector3.one;
    
    [Serializable] private struct ShakeData
    {
        public ShakeType ShakeType;
        public float Duration;
        public float Strength;
    }
    
    protected void Initialize()
    {
        if (_mesh == null) _mesh = transform;
        CaptureDefaultValues();
    }

    public void SetDefaultValues(Vector3 defaultScale)
    {
        _defaultScale = defaultScale;
        _mesh.localScale = defaultScale;
    }
    
    private void CaptureDefaultValues()
    {
        _defaultScale = _mesh.localScale; 
    }
    
    private void SetDefaultValues()
    { 
        if (_defaultScale == -Vector3.one) CaptureDefaultValues();
        
        _mesh.localScale = _defaultScale;
    }
    
    public void Shake()
    {
        SetDefaultValues();

        DOTween.Complete(_mesh);
        
        _shakeDatas.ForEach(Shake);
    }

    private void Shake(ShakeData shakeData)
    {
        switch (shakeData.ShakeType)
        {
            case ShakeType.Scale: _mesh.DOShakeScale(shakeData.Duration, shakeData.Strength).SetLink(_mesh.gameObject).SetUpdate(true); break;
            case ShakeType.Rotation: _mesh.DOShakeRotation(shakeData.Duration, shakeData.Strength).SetLink(_mesh.gameObject).SetUpdate(true); break;
        }
    }
    
    protected void OnDisable() => _mesh.DOComplete();
    protected void OnDestroy() => _mesh.DOComplete();

    private enum ShakeType
    {
        Scale,
        Rotation,
    }
}