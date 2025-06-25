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
    
    public Vector3 GetPosition(PointFollowerUI pointFollower, Vector2 preferedPosition)
    {
        if (preferedPosition.y + pointFollower.TopOffset > _upCorner.transform.localPosition.y)
        {
            preferedPosition.y = _upCorner.transform.localPosition.y - pointFollower.TopOffset;
        }
        else if (preferedPosition.y - pointFollower.BottomOffset < _downCorner.transform.localPosition.y)
        {
            preferedPosition.y = _downCorner.transform.localPosition.y + pointFollower.BottomOffset;
        }
        
        if (preferedPosition.x - pointFollower.LeftOffset < _leftCorner.transform.localPosition.x)
        {
            preferedPosition.x = _leftCorner.transform.localPosition.x + pointFollower.LeftOffset;
        }
        else if (preferedPosition.x + pointFollower.RightOffset > _rightCorner.transform.localPosition.x)
        {
            preferedPosition.x = _rightCorner.transform.localPosition.x - pointFollower.RightOffset;
        }
        
        return preferedPosition;
    }
}