using UnityEngine;

public abstract class PlacementModule : ScriptableObject
{
    public abstract bool CanBePlaced(GameObject objectToPlace, int x, int z);
    public abstract float GetHeight(int x, int z);
    public abstract Vector2Int GetPlacementPosition(int x, int z);
}