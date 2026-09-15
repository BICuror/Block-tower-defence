using Cashing;
using NaughtyAttributes;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(Collider))]

public sealed class AreaScanerController : MonoBehaviour
{
    [Inject] private DraggableSystemConfig _draggableSystemConfig;
    [Cached] private DraggableObject _draggableObject;
    [Cached] private AreaManager _areaManager;
    
    [SerializeField] private bool _autoScale;

    [Header("AreaScale")] 
    [SerializeField] private float _height = 100f;
    [SerializeField] private float _areaScaleValue = 2f;
    
    [SerializeField] private bool _overrideAdditionalScaleValue;
    [ShowIf("_overrideAdditionalScaleValue")] [SerializeField] private float _additionalScaleValue = 0.95f;
    
    [Header("Visualisation")]
    [SerializeField] private bool _hasVisualisation = true;
    [ShowIf("_hasVisualisation")] [SerializeField] private AreaVisualisation _areaVisualisation;
    private float _currentRadius;
    
    public bool HasVisualisation => _hasVisualisation;
    public AreaVisualisation AreaVisualisation => _areaVisualisation;

    private void Start()
    {
        if (_autoScale)
        {
            if (!_areaManager) _areaManager = transform.parent.GetComponent<AreaManager>();
            _areaManager.AddAreaScanerController(this);
        }

        if (_draggableObject)
        {
            _draggableObject.OnTileScaleChanged += UpdateScale;
        }
    }
    
    public void SetScale(int radius)
    {
        _currentRadius = radius;

        UpdateScale();
    }

    private void UpdateScale()
    {
        transform.localScale = GetScale();
    }

    private Vector3 GetScale()
    {
        float scale = _currentRadius * _areaScaleValue;

        if (_overrideAdditionalScaleValue) scale += _additionalScaleValue;
        else scale += _draggableSystemConfig.AdditionalAreaVisualisationSize;

        if (_draggableObject) scale += (_draggableObject.TileScale - 1);

        return new Vector3(scale, _height, scale);
    }
    
    private void OnDestroy()
    {
        if (_autoScale) _areaManager.RemoveAreaScanerController(this);
        if (_draggableObject) _draggableObject.OnTileScaleChanged -= UpdateScale;
    }
}