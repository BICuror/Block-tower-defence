using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

using Random = UnityEngine.Random;

public static class TileMap
{
    public const float RAY_HEIGHT = 10000f;
    public const float RAY_LENGTH = 10000f;

    #region HasTile

    public static bool HasTile(Vector2Int position, LayerSettingType layerSettingType)
    {
        Ray heightRay = GetRay(position);

        return HasTile(heightRay, layerSettingType);
    }
    
    public static bool HasTile(Ray heightRay, LayerSettingType layerSettingType)
    {
        return Physics.Raycast(heightRay, RAY_LENGTH, LayerService.GetLayerSetting(layerSettingType).GetLayerMask());
    }

    public static bool HasTile(Vector2Int position, LayerSettingType layerSettingType, out RaycastHit hit)
    {
        Ray heightRay = GetRay(position);

        return Physics.Raycast(heightRay, out hit, RAY_LENGTH, LayerService.GetLayerSetting(layerSettingType).GetLayerMask());
    }

    public static bool HasTile(Ray heightRay, LayerSettingType layerSettingType, out RaycastHit hit)
    {
        return Physics.Raycast(heightRay, out hit, RAY_LENGTH, LayerService.GetLayerSetting(layerSettingType).GetLayerMask());
    }

    #endregion

    #region FindSuitablePosition

    public static Vector2Int FindSuitablePositionNearby(Predicate<Vector2Int> positionValidator, Vector2Int position, int maxRadius = 10)
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

    public static RaycastHit GetHitInfo(Vector2Int position, LayerSettingType layerSettingType)
    {
        Ray heightRay = GetRay(position);

        Physics.Raycast(heightRay, out RaycastHit hit, RAY_LENGTH, LayerService.GetLayerSetting(layerSettingType).GetLayerMask());

        return hit;
    }

    public static GameObject GetHitObject(Vector2Int position, LayerSettingType layerSettingType)
    {
        Ray heightRay = GetRay(position);

        Physics.Raycast(heightRay, out RaycastHit hit, RAY_LENGTH, LayerService.GetLayerSetting(layerSettingType).GetLayerMask());

        return hit.collider.gameObject;
    }
    
    public static List<GameObject> GetHitObjects(Vector2Int position, LayerSettingType layerSettingType)
    {
        Ray heightRay = GetRay(position);

        RaycastHit[] hits = Physics.RaycastAll(heightRay, RAY_LENGTH, LayerService.GetLayerSetting(layerSettingType).GetLayerMask());
        
        List<GameObject> result = new List<GameObject>();
        
        foreach (RaycastHit raycastHit in hits)
        {
            result.Add(raycastHit.collider.gameObject);
        }

        return result;
    }

    #endregion

    #region GetTileCount

    public static int GetTileCount(Vector2Int position, LayerSettingType layerSettingType)
    {
        RaycastHit[] hits = Physics.RaycastAll(GetRay(position), RAY_LENGTH, LayerService.GetLayerSetting(layerSettingType).GetLayerMask());

        return hits.Length;
    }

    #endregion GetTileCount

    #region FindSuitablePositionsInRaduis

    public static List<Vector2Int> GetAllValidPositionsInRadius(Predicate<Vector2Int> positionValidator, Vector2Int position, int radius, int maxRadius)
    {
        List<Vector2Int> foundPositions = new();
        
        for (int currentRadius = radius; currentRadius <= maxRadius; currentRadius++)
        {
            foundPositions.AddRange(GetSuitablePositionsInRadius(positionValidator, position, currentRadius));
        }

        return foundPositions;
    }
    
    public static int CountValidPositionsInRadius(Predicate<Vector2Int> positionValidator, Vector2Int position, int radius, int maxRadius) => GetAllValidPositionsInRadius(positionValidator, position, radius, maxRadius).Count;
    
    public static List<Vector2Int> FindFurthestValidPositionsInRadius(Predicate<Vector2Int> positionValidator, Vector2Int position, int radius)
    {
        List<Vector2Int> foundPositions = new();
        
        for (int currentRadius = radius; currentRadius >= 0; currentRadius--)
        {
            foundPositions = GetSuitablePositionsInRadius(positionValidator, position, currentRadius);
            
            if (foundPositions.Count > 0) break;
        }

        return foundPositions;
    }
    
    public static List<Vector2Int> FindClosestValidPositionsInRadius(Predicate<Vector2Int> positionValidator, Vector2Int position, int radius, int maxRadius)
    {
        List<Vector2Int> foundPositions = new();
        
        for (int currentRadius = radius; currentRadius <= maxRadius; currentRadius++)
        {
            foundPositions = GetSuitablePositionsInRadius(positionValidator, position, currentRadius);
            
            if (foundPositions.Count > 0) break;
        }

        return foundPositions;
    }

    public static List<Vector2Int> GetSuitablePositionsInRadius(Predicate<Vector2Int> positionValidator, Vector2Int position, int radius = 3)
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

    #region GetNearestDraggablePlacePosition

    public static Vector3 GetNearestDraggablePlacePosition(DraggableObject draggableObject, Vector3 desiredPosition, Predicate<Vector2Int> positionValidator = null, int maxRadius = 50)
    {
        Vector2Int roundedDesiredPosition = new Vector2Int(Mathf.RoundToInt(desiredPosition.x), Mathf.RoundToInt(desiredPosition.z));

        List<Vector2Int> possiblePositions = FindClosestValidPositionsInRadius(IsValidPosition, roundedDesiredPosition, 0, maxRadius);

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

    #region GetAllRoadPositionsInRadius

    public static List<Vector2Int> GetAllRoadPositionsInRadius(Vector2Int position, bool[,] roadMap, int radius)
    {
        List<Vector2Int> possiblePositions = GetAllValidPositionsInRadius(IsARoadTile, position, 0, radius);
        
        return possiblePositions;

        bool IsARoadTile(Vector2Int searchPosition) => IsInBounds(searchPosition, roadMap) && roadMap[searchPosition.x, searchPosition.y];
    }

    #endregion

    #region GetAverageMainRoadNodeWeight

    public static float GetAverageMainRoadNodeWeight(List<Vector2Int> roadPositions, int[,] weightMap)
    {
        List<int> roadWeights = new List<int>();
            
        roadPositions.ForEach(roadPosition =>
        {
            roadWeights.Add(weightMap[roadPosition.x, roadPosition.y]);
            
            Debug.Log(weightMap[roadPosition.x, roadPosition.y]);
        });

        float averageWeight = (float)roadWeights.Average();
        
        return averageWeight;
    }

    #endregion

    #region IsInBounds

    public static bool IsInBounds(Vector2Int position, bool[,] roadMap)
    {
        return IsInBounds(position.x, position.y, roadMap);
    }

    public static bool IsInBounds(int x, int z, bool[,] roadMap)
    {
        return x >= 0 && z >= 0 && x < roadMap.GetLength(0) && z < roadMap.GetLength(1);
    }

    #endregion
    
    #region HasAValidRoadFromStartToEnd    
    
    private static readonly Vector2Int[] _checkDirections = new Vector2Int[4]
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left, 
        Vector2Int.right
    };
    
    public static bool HasAValidRoadFromStartToEnd(bool[,] roadMap, Vector2Int startingPosition, Vector2Int endPosition, int minWeight, int maxWeight, out int resultLength)
    {
        int mapSize = roadMap.GetLength(0);
        
        int[,] weightMap = new int[mapSize, mapSize];
        
        bool result = GetMinLength(startingPosition, 0, out int length);

        resultLength = length;

        return result;
        
        bool GetMinLength(Vector2Int position, int weight, out int finalWeight)
        {
            finalWeight = 0;
            
            weight++;
            weightMap[position.x, position.y] = weight;

            if (position == endPosition && weight <= maxWeight && weight >= minWeight)
            {
                finalWeight = weight;
                return true;
            }

            List<int> weights = new List<int>();

            for (int i = 0; i < _checkDirections.Length; i++)
            {
                Vector2Int checkPosition = _checkDirections[i] + position;
                
                if (!IsInBorders(checkPosition)) continue;
                
                if (roadMap[checkPosition.x, checkPosition.y])
                {
                    if (weightMap[checkPosition.x, checkPosition.y] == 0 || weightMap[checkPosition.x, checkPosition.y] > weight + 1) 
                    {
                        if (GetMinLength(checkPosition, weight, out int foundWeight))
                        {
                            weights.Add(foundWeight);
                        }
                    }   
                }
            }

            if (weights.Count == 0) return false;

            int minFoundWeight = weights.Min();

            if (minFoundWeight < maxWeight && minFoundWeight > minWeight)
            {
                finalWeight = minFoundWeight;
                return true;
            }

            return false;
        }
        
        bool IsInBorders(Vector2Int position) 
        {
            return (position.x < mapSize && position.x >= 0 && position.y < mapSize && position.y >= 0);
        }
    }
    
    #endregion
    
    #region HasTileNearby

    public static bool HasTileNearby(Vector2Int position, int radius, LayerSettingType layerSettingType, bool squareRadius = false)
    {
        for (int x = -radius; x <= radius; x++)
        {
            for (int z = -radius; z <= radius; z++)
            {
                Vector2Int checkPosition = new Vector2Int(x, z);
                
                if (squareRadius || checkPosition.magnitude <= radius)
                {
                    if (HasTile(position + checkPosition, layerSettingType))
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