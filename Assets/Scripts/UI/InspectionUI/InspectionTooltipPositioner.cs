using UnityEngine;

public sealed class InspectionTooltipPositioner : MonoBehaviour
{
    private static InspectionTooltipPositioner _instance;
    
    public static InspectionTooltipPositioner Instance => _instance;
    
    [SerializeField] private RectTransform _leftCorner;
    [SerializeField] private RectTransform _rightCorner;
    [SerializeField] private RectTransform _upCorner;
    [SerializeField] private RectTransform _downCorner;
    
    private void Awake()
    {
        _instance = this;
    } 
    
    public Vector3 GetPosition(PointFollowingCanvasUIElement.PointFollowingElementOffsetContainer offsetsContainer, Vector2 preferredPosition)
    {
        if (preferredPosition.y + offsetsContainer.TopOffset > _upCorner.transform.localPosition.y)
        {
            preferredPosition.y = _upCorner.transform.localPosition.y - offsetsContainer.TopOffset;
        }
        else if (preferredPosition.y - offsetsContainer.BottomOffset < _downCorner.transform.localPosition.y)
        {
            preferredPosition.y = _downCorner.transform.localPosition.y + offsetsContainer.BottomOffset;
        }
        
        if (preferredPosition.x - offsetsContainer.LeftOffset < _leftCorner.transform.localPosition.x)
        {
            preferredPosition.x = _leftCorner.transform.localPosition.x + offsetsContainer.LeftOffset;
        }
        else if (preferredPosition.x + offsetsContainer.RightOffset > _rightCorner.transform.localPosition.x)
        {
            preferredPosition.x = _rightCorner.transform.localPosition.x - offsetsContainer.RightOffset;
        }
        
        return preferredPosition;
    }
}