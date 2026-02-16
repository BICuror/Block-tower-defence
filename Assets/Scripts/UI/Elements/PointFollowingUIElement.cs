using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public sealed class PointFollowingUIElement : MonoBehaviour
{
    [SerializeField] private List<RectTransform> _subPanels;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Vector2 _staicOffset;
    
    private PointFollowingElementOffsetContainer _pointFollowingElementOffsetContainer;
    private Func<Vector2> _provideDynamicOffset = () => Vector2.zero;
    private Camera _mainCamera;
    private Transform _target;
    
    private void Awake()
    {
        _mainCamera = Camera.main;
        CalculateOffsets();
    }
    
    public void SetDynamicOffsetProvider(Func<Vector2> provideDynamicOffset) => _provideDynamicOffset = provideDynamicOffset;
    
    public void SetTarget(Transform target) => _target = target;
    
    private void CalculateOffsets()
    {
        float xSize = _rectTransform.sizeDelta.x / 2 * transform.localScale.x;
        float ySize = _subPanels.Max(panel => panel.sizeDelta.y) / 2 * transform.localScale.y;
        
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

        Vector2 preferredUIPosition = resultPoint + _staicOffset + _provideDynamicOffset.Invoke();
        
        Vector2 finalPosition = InspectionTooltipPositioner.Instance.GetPosition(_pointFollowingElementOffsetContainer, preferredUIPosition);
        
        _rectTransform.localPosition = finalPosition;
    }

    public record PointFollowingElementOffsetContainer
    {
        public float TopOffset;
        public float BottomOffset;
        public float RightOffset;
        public float LeftOffset;
    }
}