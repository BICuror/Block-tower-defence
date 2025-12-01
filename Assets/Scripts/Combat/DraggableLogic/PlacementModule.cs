using UnityEngine;

public abstract class PlacementModule : ScriptableObject
{
    [SerializeField] protected float AdditionalPlacementHeight = 0.5f;
    
    public abstract bool CanBePlaced(Vector2Int position);
    public abstract float GetHeight(Vector2Int position);
    public abstract Vector2Int GetPlacementPosition(Vector2Int position);
}