using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public abstract class PointFollowingCanvasUIElement : CanvasGameUIElement
{
    [SerializeField] private bool _layoutControllers = true;
    
    [Header("Positioning")]
    [SerializeField] private RectTransform _inspectablePosition;
    [SerializeField] private float _yOffset;
    
    [Header("FadeAnimation")]
    [SerializeField] private CanvasGroup _mainGroup;
    [SerializeField] private float _fadeDuration = 0.2f;
    
    private PointFollowingElementOffsetContainer _pointFollowingElementOffsetContainer;
    private Vector2 _targetElementDirection;
    private Camera _mainCamera;
    private Transform _target;
    private bool _isActive;

    public Vector2 TargetElementDirection => _targetElementDirection;
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
        await _mainGroup.DOFade(0f, _fadeDuration).OnComplete(() => gameObject.SetActive(false)).SetLink(_mainGroup.gameObject).AsyncWaitForCompletion();
    }

    protected async UniTask RebuildLayoutAndCalculateOffsets()
    {
        await RebuildLayout();

        CalculateOffsets();
    }

    protected void SetTarget(Transform target) => _target = target;
    
    private void CalculateOffsets()
    {
        RectTransform rect = transform as RectTransform;

        float xSize = rect.sizeDelta.x / 2;
        float ySize = rect.sizeDelta.y / 2;

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
        Vector2 targetScreenPosition = RectTransformUtility.WorldToScreenPoint(_mainCamera, _target.position);
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent as RectTransform, targetScreenPosition, null, out Vector2 resultPoint);
        
        Vector2 preferredUIPosition = resultPoint - new Vector2(_inspectablePosition.anchoredPosition.x, - (_yOffset + _pointFollowingElementOffsetContainer.BottomOffset));
        
        Vector2 finalPosition = InspectionTooltipPositioner.Instance.GetPosition(_pointFollowingElementOffsetContainer, preferredUIPosition);
        
        transform.GetComponent<RectTransform>().localPosition = finalPosition;

        _targetElementDirection = finalPosition - targetScreenPosition;
    }

    public record PointFollowingElementOffsetContainer
    {
        public float TopOffset;
        public float BottomOffset;
        public float RightOffset;
        public float LeftOffset;
    }
}