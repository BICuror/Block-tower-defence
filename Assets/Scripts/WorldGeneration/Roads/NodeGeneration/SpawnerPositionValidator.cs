using UnityEngine;

public abstract class SpawnerPositionValidator : ScriptableObject
{
    public abstract bool IsValidPosition(int currentX, int maxX, int currentZ, int maxZ);
}
