using NaughtyAttributes;
using UnityEngine;
using DG.Tweening;
using Zenject;

public abstract class AreaVisualisation : MonoBehaviour
{
    [Inject] [SerializeField] private DraggableSystemConfig _draggableSystemConfig;
    
    [Header("Visualisation")]
    [SerializeField] private Transform _visualisationTransform;
    private TokenContainer _areaActiveVisualisationTokenContainer = new();
    private float _defaultScale = 1f;

    public float DefaultScale => _defaultScale;
    protected abstract Vector3 DisabledScale { get; }
    protected abstract Vector3 EnabledScale { get; }

    private void Start()
    {
        _visualisationTransform.localScale = DisabledScale;

        if (transform.parent.TryGetComponent(out HoverableObject hoverableObject)) SubscribeToHoverable(hoverableObject);
    }
    
    public void SetDefaultScale(float scale) => _defaultScale = scale;
    
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
        _areaActiveVisualisationTokenContainer.AddToken();
        
        if (_areaActiveVisualisationTokenContainer.TokenCount > 1) return;
        
        _visualisationTransform.DOKill();
        _visualisationTransform.gameObject.SetActive(true);
        
        _visualisationTransform.DOScale(EnabledScale, _draggableSystemConfig.AreaVisualisationAppearDuration).SetEase(_draggableSystemConfig.AreaVisualisationAppearCurve);
    }

    public void DisableVisualisation()
    {
        _areaActiveVisualisationTokenContainer.RemoveToken();
        
        if (!_areaActiveVisualisationTokenContainer.IsEmpty) return;
        
        _visualisationTransform.DOKill();
        
        _visualisationTransform.DOScale(DisabledScale, _draggableSystemConfig.AreaVisualisationDisappearDuration).SetEase(_draggableSystemConfig.AreaVisualisationDisappearCurve).OnComplete(() =>
        {
            _visualisationTransform.gameObject.SetActive(false);
        });
    }

    private void OnDestroy()
    {
        if (transform.parent.TryGetComponent(out HoverableObject hoverableObject)) UnsubscribeFromHoverable(hoverableObject);
    }
}