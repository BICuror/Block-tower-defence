using DG.Tweening;
using UnityEngine;

public abstract class Shaker : MonoBehaviour
{
    [Header("ShakeSettings")]
    [SerializeField] private float _shakeDuration = 0.5f;
    protected float ShakeDuration {get => _shakeDuration; set => _shakeDuration = value;}
    [SerializeField] private float _shakeStrength = 0.3f;

    [Header("Links")]
    [SerializeField] protected Transform _mesh;
    private Vector3 _defaultScale;
    private Tween _currentTween;

    protected void Shake()
    {
        SetDefaultScale();

        if (_currentTween != null && _currentTween.IsPlaying())
        {
            _currentTween.Complete();
        }
        
        _currentTween = _mesh.DOShakeScale(_shakeDuration, _shakeStrength);
    }

    protected void CaptureDefaultScale() => _defaultScale = _mesh.localScale;

    private void SetDefaultScale()
    {
        _mesh.localScale = _defaultScale;
    }

    private void OnDisable() => _currentTween.Complete();
    private void OnDestroy() => _currentTween.Kill();
}
