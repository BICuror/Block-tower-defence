using NaughtyAttributes;
using DG.Tweening;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(Collider))]

public sealed class AreaScanerController : MonoBehaviour
{
    [Inject] private DraggableSystemConfig _draggableSystemConfig;
    private AreaManager _areaManager;
    
    [SerializeField] private bool _autoScale;

    [Header("AreaScale")] 
    [SerializeField] private bool _overrideAdditionalScaleValue;
    [ShowIf("_overrideAdditionalScaleValue")] [SerializeField] private float _additionalScaleValue = 0.95f;
    [SerializeField] private float _areaScaleValue = 2f;
    
    [Header("Visualisation")]
    [SerializeField] private bool _hasVisualisation = true;
    [ShowIf("_hasVisualisation")] [SerializeField] private Transform _visualisationTransform;
    [ShowIf("_hasVisualisation")] [SerializeField] private float _height = 100f;
    private TokenContainer _areaActiveVisualisationTokenContainer = new();
    private float _currentRadius;
    
    private Vector3 DisabledScale => new (0f, _height, 0f);
    private Vector3 EnabledScale => new (1f, _height, 1f);

    private void Start()
    {
        if (_hasVisualisation)
        {
            _visualisationTransform.localScale = DisabledScale;

            if (transform.parent.TryGetComponent(out HoverableObject hoverableObject)) SubscribeToHoverable(hoverableObject);
        }

        if (_autoScale)
        {
            _areaManager = transform.parent.GetComponent<AreaManager>();
            _areaManager.AddAreaScanerController(this);
        }
    }
    
    public void SetScale(int radius)
    {
        _currentRadius = radius;
        
        transform.localScale = GetScale();
    }

    private Vector3 GetScale()
    {
        float scale = _currentRadius * _areaScaleValue;

        if (_overrideAdditionalScaleValue) scale += _additionalScaleValue;
        else scale += _draggableSystemConfig.AdditionalAreaVisualisationSize;

        return new Vector3(scale, _height, scale);
    }
    
    #region Visualisation

    public void SubscribeToHoverable(HoverableObject hoverableObject)
    {
        hoverableObject.HoverEntered.AddListener(EnableVisualisation);
        hoverableObject.HoverExited.AddListener(DisableVisualisation);
    }

    public void UnsubscribeFromHoverable(HoverableObject hoverableObject)
    {
        hoverableObject.HoverEntered.RemoveListener(EnableVisualisation);
        hoverableObject.HoverExited.RemoveListener(DisableVisualisation);
    }
    
    public void EnableVisualisation()
    {
        if (!_hasVisualisation) return;
        
        _areaActiveVisualisationTokenContainer.AddToken();
        
        if (_areaActiveVisualisationTokenContainer.TokenCount > 1) return;
        
        _visualisationTransform.DOKill();
        _visualisationTransform.gameObject.SetActive(true);
        
        _visualisationTransform.DOScale(EnabledScale, _draggableSystemConfig.AreaVisualisationAppearDuration).SetEase(_draggableSystemConfig.AreaVisualisationAppearCurve);
    }

    public void DisableVisualisation()
    {
        if (!_hasVisualisation) return;
        
        _areaActiveVisualisationTokenContainer.RemoveToken();
        
        if (!_areaActiveVisualisationTokenContainer.IsEmpty) return;
        
        _visualisationTransform.DOKill();
        
        _visualisationTransform.DOScale(DisabledScale, _draggableSystemConfig.AreaVisualisationDisappearDuration).SetEase(_draggableSystemConfig.AreaVisualisationDisappearCurve).OnComplete(() =>
        {
            _visualisationTransform.gameObject.SetActive(false);
        });
    }
    
    #endregion
    
    private void OnDestroy()
    {
        if (_autoScale) _areaManager.RemoveAreaScanerController(this);
        
        if (transform.parent.TryGetComponent(out HoverableObject hoverableObject)) UnsubscribeFromHoverable(hoverableObject);
    }
}