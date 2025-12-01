using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using System;

public abstract class Shaker : MonoBehaviour
{
    [Header("ShakeSettings")] 
    [SerializeField] private List<ShakeData> _shakeDatas;

    [Header("Links")]
    [SerializeField] protected Transform _mesh;
    private Vector3 _defaultScale;
    
    [Serializable] private struct ShakeData
    {
        public ShakeType ShakeType;
        public float Duration;
        public float Strength;
    }

    protected void Awake()
    {
        if (_mesh == null) _mesh = transform;
        GetDefaultValues();
    }
    
    private void GetDefaultValues()
    {
        _defaultScale = _mesh.localScale; 
    }
    
    private void SetDefaultValues()
    { 
        _mesh.localScale = _defaultScale;
    }
    
    protected void Shake()
    {
        SetDefaultValues();

        DOTween.Complete(_mesh);
        
        _shakeDatas.ForEach(shakeData =>
        {
            Shake(shakeData);    
        });
    }

    private void Shake(ShakeData shakeData)
    {
        switch (shakeData.ShakeType)
        {
            case ShakeType.Scale: _mesh.DOShakeScale(shakeData.Duration, shakeData.Strength); break;
            case ShakeType.Rotation: _mesh.DOShakeRotation(shakeData.Duration, shakeData.Strength); break;
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