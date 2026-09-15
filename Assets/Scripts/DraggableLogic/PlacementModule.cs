using UnityEngine;

public abstract class PlacementModule : ScriptableObject
{
    private const int MAX_DISTANCE_FROM_CENTER = 18;
    private const int CENTER_INDEX = 15;
    
    [SerializeField] protected float AdditionalPlacementHeight = 0.5f;
    
    public abstract bool CanBePlaced(Vector2 position, int tileScale);
    public abstract float GetHeight(Vector2 position);
    public abstract Vector2 GetPlacementPosition(Vector2 position, int tileScale);
    
    protected bool IsValidPosition(Vector2Int position)
    {
        return Vector2Int.Distance(position, new Vector2Int(CENTER_INDEX, CENTER_INDEX)) <= MAX_DISTANCE_FROM_CENTER;
    }
}