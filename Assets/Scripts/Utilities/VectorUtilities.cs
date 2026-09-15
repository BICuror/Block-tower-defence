using UnityEngine;

public static class VectorUtilities
{
    public static Vector2 ToVector2(this Vector3 vector)
    {
        Vector2 resultVector = new Vector2(vector.x, vector.z);
        
        return resultVector;
    }
    
    public static Vector3 ToVector3(this Vector2 vector, float height)
    {
        Vector3 resultVector = new Vector3(vector.x, height, vector.y);
        
        return resultVector;
    }
    
    public static Vector2Int ToIntVector(this Vector2 vector)
    {
        Vector2Int newVector2Int = new Vector2Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y));
        
        return newVector2Int;
    }
}