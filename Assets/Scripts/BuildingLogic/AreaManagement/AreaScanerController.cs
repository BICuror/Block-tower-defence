using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Collider))]

public sealed class AreaScanerController : MonoBehaviour
{
    [SerializeField] private Transform _visualisationTransform;
    [SerializeField] private float _height = 100f;
    private float _currentRadius;
    
    private Vector3 DisabledScale => new (0f, _height, 0f);
    private Vector3 EnabledScale => new (1f, _height, 1f);

    private void Awake()
    {
        _visualisationTransform.localScale = DisabledScale;
    }
    
    public void SetScale(int radius)
    {
        _currentRadius = radius;
        
        transform.localScale = GetScale();
    }

    public void EnableVisualisation(float duration, AnimationCurve curve)
    {
        _visualisationTransform.DOKill();
        _visualisationTransform.gameObject.SetActive(true);
        
        _visualisationTransform.DOScale(EnabledScale, duration).SetEase(curve);
    }

    public void DisableVisualisation(float duration, AnimationCurve curve)
    {
        _visualisationTransform.DOKill();
        
        _visualisationTransform.DOScale(DisabledScale, duration).SetEase(curve).OnComplete(() => _visualisationTransform.gameObject.SetActive(false));
    }
    
    private Vector3 GetScale()
    {
        float scale = _currentRadius * 2f + 0.95f;

        return new Vector3(scale, _height, scale);
    }
}