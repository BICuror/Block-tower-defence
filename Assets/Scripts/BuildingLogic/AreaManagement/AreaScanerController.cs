using NaughtyAttributes;
using DG.Tweening;
using UnityEngine;
using Cashing;

[RequireComponent(typeof(Collider))]

public sealed class AreaScanerController : MonoBehaviour
{
    [Cached] private AreaManager _areaManager;
    [SerializeField] private bool _autoScale;
    [SerializeField] private float _additionalScaleValue = 0.95f;
    
    [Header("Visualisation")]
    [SerializeField] private bool _hasVisualisation = true;
    [ShowIf("_hasVisualisation")] [SerializeField] private Transform _visualisationTransform;
    [ShowIf("_hasVisualisation")] [SerializeField] private float _height = 100f;
    private float _currentRadius;
    
    private Vector3 DisabledScale => new (0f, _height, 0f);
    private Vector3 EnabledScale => new (1f, _height, 1f);

    private void Start()
    {
        if (_hasVisualisation) _visualisationTransform.localScale = DisabledScale;
        
        if (_autoScale) _areaManager.AddAreaScanerController(this);
    }
    
    public void SetScale(int radius)
    {
        _currentRadius = radius;
        
        transform.localScale = GetScale();
    }

    public void EnableVisualisation(float duration, AnimationCurve curve)
    {
        if (!_hasVisualisation) return;
        
        _visualisationTransform.DOKill();
        _visualisationTransform.gameObject.SetActive(true);
        
        _visualisationTransform.DOScale(EnabledScale, duration).SetEase(curve);
    }

    public void DisableVisualisation(float duration, AnimationCurve curve)
    {
        if (!_hasVisualisation) return;
        
        _visualisationTransform.DOKill();
        
        _visualisationTransform.DOScale(DisabledScale, duration).SetEase(curve).OnComplete(() => _visualisationTransform.gameObject.SetActive(false));
    }
    
    private Vector3 GetScale()
    {
        float scale = _currentRadius * 2f + _additionalScaleValue;

        return new Vector3(scale, _height, scale);
    }

    private void OnDestroy()
    {
        if (_autoScale) _areaManager.RemoveAreaScanerController(this);
    }
}