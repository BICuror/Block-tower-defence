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
    protected Vector3 DefaultScale = -Vector3.one;
    
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
        DefaultScale = defaultScale;
        _mesh.localScale = defaultScale;
    }
    
    private void CaptureDefaultValues()
    {
        DefaultScale = _mesh.localScale; 
    }
    
    private void SetDefaultValues()
    { 
        if (DefaultScale == -Vector3.one) CaptureDefaultValues();
        
        _mesh.localScale = DefaultScale;
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