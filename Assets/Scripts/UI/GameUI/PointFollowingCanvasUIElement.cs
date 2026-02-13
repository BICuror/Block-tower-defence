using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class PointFollowingCanvasUIElement : CanvasGameUIElement
{
    [SerializeField] private bool _desrtroyOnDisable = true;
    [SerializeField] private RectTransform _rectTransform;
    
    [Header("Positioning")]
    [SerializeField] private List<RectTransform> _subPanels;
    [FormerlySerializedAs("_offset")] [SerializeField] private Vector2 _staicOffset;
    
    [Header("FadeAnimation")]
    [SerializeField] private CanvasGroup _mainGroup;
    [SerializeField] private float _fadeDuration = 0.2f;
    
    private PointFollowingElementOffsetContainer _pointFollowingElementOffsetContainer;
    private Camera _mainCamera;
    private Transform _target;
    private bool _isActive;
    
    protected virtual Vector2 DynamicOffset => Vector2.zero;
    protected RectTransform MainRectTransform => _rectTransform;
    
    public bool IsActive => _isActive;
    
    private void Awake()
    {
        _mainCamera = Camera.main;
        _mainGroup.interactable = false;
        _mainGroup.alpha = 0f;
        CalculateOffsets();
    }
    
    public async UniTask Enable()
    {
        if (_isActive) return;
        _mainGroup.DOKill();
        
        _isActive = true;
        gameObject.SetActive(true);
        _mainGroup.interactable = true;
        await _mainGroup.DOFade(1f, _fadeDuration).SetLink(_mainGroup.gameObject).AsyncWaitForCompletion();
    }
    
    public async UniTask Disable()
    {
        if (!_isActive) return;
        _mainGroup.DOKill();
        
        _isActive = false;
        _mainGroup.interactable = false;
        await _mainGroup.DOFade(0f, _fadeDuration).OnComplete(DisableGameObject).SetLink(_mainGroup.gameObject).AsyncWaitForCompletion();
    }

    private void DisableGameObject()
    {
        if (!gameObject) return;

        _mainGroup.DOKill();

        gameObject.SetActive(false);
    }


    protected void SetTarget(Transform target) => _target = target;
    
    private void CalculateOffsets()
    {
        RectTransform rect = transform as RectTransform;

        float xSize = rect.sizeDelta.x / 2;
        float ySize = _subPanels.Max(panel => panel.sizeDelta.y) / 2;
        
        _pointFollowingElementOffsetContainer = new()
        {
            RightOffset = xSize,
            LeftOffset = xSize,
            TopOffset = ySize,
            BottomOffset = ySize
        };
    }
    
    private void Update()
    {
        UpdatePosition();
    } 
    
    private void UpdatePosition()
    {
        if (!_target) return;
        
        CalculateOffsets();
        
        Vector2 targetScreenPosition = RectTransformUtility.WorldToScreenPoint(_mainCamera, _target.position);
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent as RectTransform, targetScreenPosition, null, out Vector2 resultPoint);

        Vector2 preferredUIPosition = resultPoint + _staicOffset + DynamicOffset;
        
        Vector2 finalPosition = InspectionTooltipPositioner.Instance.GetPosition(_pointFollowingElementOffsetContainer, preferredUIPosition);
        
        _rectTransform.localPosition = finalPosition;
    }

    private void OnDestroy()
    {
        _mainGroup.DOKill();
    }

    public record PointFollowingElementOffsetContainer
    {
        public float TopOffset;
        public float BottomOffset;
        public float RightOffset;
        public float LeftOffset;
    }
}