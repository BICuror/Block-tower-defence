using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class PointFollowerUI : MonoBehaviour
{
    [SerializeField] private RectTransform _inspectablePosition;
    private RectTransform _parentRect;
    private RectTransform _rect;
    private Transform _target;
    
    public float TopOffset => _rect.sizeDelta.y / 2 ;
    public float BottomOffset => _rect.sizeDelta.y / 2 ;
    public float RightOffset => _rect.sizeDelta.x / 2 ;
    public float LeftOffset => _rect.sizeDelta.x / 2 ;

    private void Awake()
    {
        _rect = transform as RectTransform;
        _parentRect = transform.parent as RectTransform;
    }
    
    public void SetTarget(Transform target)
    {
        _target = target;
        
        UpdatePosition();
    }

    private void Update() => UpdatePosition();
    
    private async void UpdatePosition()
    {
        Vector2 targetScreenPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, _target.position);
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_parentRect, targetScreenPosition, null, out Vector2 resultPoint);
        
        Vector2 preferedUIPosition = resultPoint - _inspectablePosition.anchoredPosition;
        
        Vector2 finalPosition = InspectionTooltipPositioner.Instance.GetPosition(this, preferedUIPosition);
        
        transform.GetComponent<RectTransform>().localPosition = finalPosition;
    }
}