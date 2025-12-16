using System.Collections.Generic;
using UnityEngine;
using System;

using Random = UnityEngine.Random;

public static class TileMap
{
    private const float RAY_HEIGHT = 10000f;
    private const float RAY_LENGTH = 10000f;

    #region HasTile

    public static bool HasTile(Vector2Int position, LayerSetting layerSetting)
    {
        Ray heightRay = GetRay(position);

        return Physics.Raycast(heightRay, RAY_LENGTH, layerSetting.GetLayerMask());
    }

    public static bool HasTile(Vector2Int position, LayerSetting layerSetting, out RaycastHit hit)
    {
        Ray heightRay = GetRay(position);

        return Physics.Raycast(heightRay, out hit, RAY_LENGTH, layerSetting.GetLayerMask());
    }

    public static bool HasTile(Ray heightRay, LayerSetting layerSetting, out RaycastHit hit)
    {
        return Physics.Raycast(heightRay, out hit, RAY_LENGTH, layerSetting.GetLayerMask());
    }

    public static bool HasTile(Ray heightRay, LayerSetting layerSetting)
    {
        return Physics.Raycast(heightRay, RAY_LENGTH, layerSetting.GetLayerMask());
    }

    #endregion

    #region FindSuitablePosition

    public static Vector2Int FindSuitablePositionNearby(Predicate<Vector2Int> positionValidator, Vector2Int position,
        int maxRadius = 10)
    {
        int centerX = position.x;
        int centerZ = position.y;

        for (int radius = 1; radius <= maxRadius; radius++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                for (int z = -radius; z <= radius; z++)
                {
                    Vector2Int checkPosition = new Vector2Int(centerX + x, centerZ + z);

                    if (positionValidator.Invoke(checkPosition))
                    {
                        return checkPosition;
                    }
                }
            }
        }

        throw new Exception($"No suitable position found in {maxRadius} tile radius");
    }

    #endregion

    #region GetHit

    public static RaycastHit GetHitInfo(Vector2Int position, LayerSetting layerSetting)
    {
        Ray heightRay = GetRay(position);

        Physics.Raycast(heightRay, out RaycastHit hit, RAY_LENGTH, layerSetting.GetLayerMask());

        return hit;
    }

    public static GameObject GetHitObject(Vector2Int position, LayerSetting layerSetting)
    {
        Ray heightRay = GetRay(position);

        Physics.Raycast(heightRay, out RaycastHit hit, RAY_LENGTH, layerSetting.GetLayerMask());

        return hit.collider.gameObject;
    }

    #endregion

    #region GetTileCount

    public static int GetTileCount(Vector2Int position, LayerSetting layerSetting)
    {
        RaycastHit[] hits = Physics.RaycastAll(GetRay(position), RAY_LENGTH, layerSetting.GetLayerMask());

        return hits.Length;
    }

    #endregion GetTileCount

    #region FindSuitablePositionsInRaduis
    
    public static List<Vector2Int> ForceGetSuitablePositionsInRadius(Predicate<Vector2Int> positionValidator, Vector2Int position, int radius = 3)
    {
        List<Vector2Int> foundPositions = GetSuitablePositionsInRadius(positionValidator, position, radius);

        if (foundPositions.Count == 0)
        {
            int modifiedRadius = radius;

            while (modifiedRadius <= 13 && foundPositions.Count == 0)
            {
                modifiedRadius++;

                foundPositions = GetSuitablePositionsInRadius(positionValidator, position, modifiedRadius);

                if (foundPositions.Count > 0)
                {
                    return foundPositions;
                }
            }

            return GetSuitablePositionsInRadius((Vector2Int _) => true, position, radius);
        }

        return foundPositions;
    }

    public static List<Vector2Int> GetSuitablePositionsInRadius(Predicate<Vector2Int> positionValidator,
        Vector2Int position, int radius = 3)
    {
        List<Vector2Int> suitablePositions = new List<Vector2Int>();

        int xMinPosition = position.x - radius;
        int xMaxPosition = position.x + radius;

        int zMinPosition = position.y - radius;
        int zMaxPosition = position.y + radius;

        for (int x = xMinPosition; x <= xMaxPosition; x++)
        {
            for (int z = zMinPosition; z <= zMaxPosition; z++)
            {
                if (x == xMinPosition || z == zMinPosition || x == xMaxPosition || z == zMaxPosition)
                {
                    Vector2Int checkPosition = new Vector2Int(x, z);

                    if (positionValidator.Invoke(checkPosition))
                    {
                        suitablePositions.Add(checkPosition);
                    }
                }
            }
        }

        return suitablePositions;
    }

    #endregion

    #region GetNearestPlacePosition

    public static Vector3 GetNearestPlacePosition(DraggableObject draggableObject, Vector3 desiredPosition,
        Predicate<Vector2Int> positionValidator = null)
    {
        Vector2Int roundedDesiredPosition =
            new Vector2Int(Mathf.RoundToInt(desiredPosition.x), Mathf.RoundToInt(desiredPosition.z));

        List<Vector2Int> possiblePositions =
            ForceGetSuitablePositionsInRadius(IsValidPosition, roundedDesiredPosition, 0);

        Vector2Int finalPosition = possiblePositions[Random.Range(0, possiblePositions.Count)];

        float height = draggableObject.GetPlacementModule().GetHeight(finalPosition);

        return new Vector3(finalPosition.x, height, finalPosition.y);

        bool IsValidPosition(Vector2Int position)
        {
            if (!draggableObject.GetPlacementModule().CanBePlaced(position)) return false;

            return positionValidator == null || positionValidator.Invoke(position);
        }
    }

    #endregion

    #region HasTileNearby

    public static bool HasTileNearby(Vector2Int position, int radius, LayerSetting layerSetting, bool squareRadius = false)
    {
        for (int x = -radius; x <= radius; x++)
        {
            for (int z = -radius; z <= radius; z++)
            {
                Vector2Int checkPosition = new Vector2Int(x, z);
                
                if (squareRadius || checkPosition.magnitude <= radius)
                {
                    if (HasTile(position + checkPosition, layerSetting))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    #endregion
    
    private static Ray GetRay(Vector2Int position)
    {
        Vector3 rayPosition = new Vector3(position.x, RAY_HEIGHT, position.y);
        
        return new Ray(rayPosition, Vector3.down);
    }
}