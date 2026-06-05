using UnityEngine;

public abstract class PlacementModule : ScriptableObject
{
    private const int MAX_DISTANCE_FROM_CENTER = 18;
    private const int CENTER_INDEX = 15;
    
    [SerializeField] protected float AdditionalPlacementHeight = 0.5f;
    
    public abstract bool CanBePlaced(Vector2Int position);
    public abstract float GetHeight(Vector2Int position);
    public abstract Vector2Int GetPlacementPosition(Vector2Int position);

    protected bool IsValidPosition(Vector2Int position)
    {
        return Vector2Int.Distance(position, new Vector2Int(CENTER_INDEX, CENTER_INDEX)) <= MAX_DISTANCE_FROM_CENTER;
    }
}