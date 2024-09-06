using UnityEngine;

public abstract class PlacementCondition : ScriptableObject
{
    public abstract bool IsSatisfied(GameObject objectToPlace, int x, int y);
}