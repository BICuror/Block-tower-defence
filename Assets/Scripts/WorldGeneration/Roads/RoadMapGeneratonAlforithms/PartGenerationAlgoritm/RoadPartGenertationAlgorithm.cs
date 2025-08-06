using System.Collections.Generic;
using WorldGeneration;
using System.Linq;
using UnityEngine;
using System;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "RoadPartGenertationAlgorithm", menuName = "Generation/RoadMapGeneratoionAlgorithm/RoadPartGenertationAlgorithm")]

public sealed class RoadPartGenertationAlgorithm : RoadGenerationAlgorithm
{
    [SerializeField] private int _maxLength;
    [SerializeField] private int _minLength;
    [SerializeField] private float _snapToEndPositionDistance = 5;
    [SerializeField] private List<RoadPartData> _roadPartDatas;
    
    public override bool TryGenerateRoadMap()
    {
        return IterateNextRoadStep(StartPosition);
    }
    
    private bool IterateNextRoadStep(Vector2Int currentPosition)
    {
        List<RoadPartData> roadPartDatas = _roadPartDatas.OrderBy(item => Random.Range(0, _roadPartDatas.Count)).ToList();
        
        for (int roadPartIndex = 0; roadPartIndex < roadPartDatas.Count; roadPartIndex++)
        {
            RoadPartData partData = roadPartDatas[roadPartIndex];

            List<RoadPartTileState[,]> rotatedGrids = GetAllRotatedGridParts(partData.GetTileGrid());
            
            rotatedGrids = rotatedGrids.OrderBy(item => Random.Range(0, rotatedGrids.Count)).ToList();

            for (int gridRotationIndex = 0; gridRotationIndex < rotatedGrids.Count; gridRotationIndex++)
            {
                Vector2Int offset = currentPosition - GetLocalEntrancePosition(rotatedGrids[gridRotationIndex]);

                if (TryApplyGrid(rotatedGrids[gridRotationIndex], offset))
                { 
                    Vector2Int endPosition = offset + GetLocalExitPosition(rotatedGrids[gridRotationIndex]);
                    
                    float distanceToEnd = Vector2.Distance(endPosition, EndPosition);
                    
                    if (HasAValidRoadFromStartToEnd(StartPosition, EndPosition, _minLength, _maxLength, out int resultLength))
                    {
                        Debug.Log(resultLength);
                        return true;
                    }
                    
                    if (!HasAValidRoadFromStartToEnd(StartPosition, endPosition, int.MinValue, int.MaxValue, out int currentPathLength))
                    {
                        RemoveGrid(rotatedGrids[gridRotationIndex], offset);
                        continue;
                    }

                    if (distanceToEnd <= _snapToEndPositionDistance && currentPathLength > _minLength && currentPathLength < _maxLength + distanceToEnd)
                    {
                        ConnectPoints(endPosition, EndPosition);
                        Debug.Log(currentPathLength + distanceToEnd);
                        return true;
                    }
                    
                    if (currentPathLength > _maxLength)
                    {
                        RemoveGrid(rotatedGrids[gridRotationIndex], offset);
                        continue;
                    }
                    
                    if (IterateNextRoadStep(endPosition))
                    {
                        return true;
                    }
                    
                    RemoveGrid(rotatedGrids[gridRotationIndex], offset);
                }
            }
        }

        return false;
    }

    private bool HasAValidRoadFromStartToEnd(Vector2Int startingPosition, Vector2Int endPosition, int minWeight, int maxWeight, out int resultLength)
    {
        int[,] weightMap = new int[IslandData.IslandSize, IslandData.IslandSize];
        
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

            for (int i = 0; i < CheckDirections.Length; i++)
            {
                Vector2Int checkPosition = CheckDirections[i] + position;
                
                if (!IsInBorders(checkPosition)) continue;
                
                if (RoadMap[checkPosition.x, checkPosition.y] || TempRoadmap[checkPosition.x, checkPosition.y])
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
    }
    
    private bool TryApplyGrid(RoadPartTileState[,] roadPartGrid, Vector2Int offset)
    {
        if (OverlapsWithExistingMap(roadPartGrid, offset)) return false;
        
        int gridSize = roadPartGrid.GetLength(0);
        
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                int xCheck = x + offset.x;
                int zCheck = z + offset.y;

                if (roadPartGrid[x, z] == RoadPartTileState.Exit || roadPartGrid[x, z] == RoadPartTileState.Solid)
                {
                    TempRoadmap[xCheck, zCheck] = true;
                }
            }
        }

        return true;
    }
    
    private bool OverlapsWithExistingMap(RoadPartTileState[,] roadPartGrid, Vector2Int offset)
    {
        int gridSize = roadPartGrid.GetLength(0);

        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                int xCheck = x + offset.x;
                int zCheck = z + offset.y;

                if (roadPartGrid[x, z] == RoadPartTileState.Empty || roadPartGrid[x, z] == RoadPartTileState.Entrance) continue;

                if (roadPartGrid[x, z] == RoadPartTileState.EmptySpaceRequired)
                {
                    if (!IsInBorders(new Vector2Int(xCheck, zCheck))) continue;
                }
                else
                {
                    if (!IsInBorders(new Vector2Int(xCheck, zCheck))) return true;
                }
                
                if (RoadMap[xCheck, zCheck] || TempRoadmap[xCheck, zCheck]) return true;
            }
        }

        return false;        
    }
    
    private void RemoveGrid(RoadPartTileState[,] roadPartGrid, Vector2Int offset)
    {
        int gridSize = roadPartGrid.GetLength(0);
        
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                int xCheck = x + offset.x;
                int zCheck = z + offset.y;

                if (roadPartGrid[x, z] == RoadPartTileState.Solid || roadPartGrid[x, z] == RoadPartTileState.Exit)
                {
                    TempRoadmap[xCheck, zCheck] = false;
                }
            }
        }
    }

    #region GridManipulation

    private Vector2Int GetLocalEntrancePosition(RoadPartTileState[,] roadGrid)
    {
        int gridSize = roadGrid.GetLength(0);

        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                if (roadGrid[x, z] == RoadPartTileState.Entrance) return new Vector2Int(x, z);
            }
        }
        
        throw new Exception("Couldn't find entrance");
    }
    
    private Vector2Int GetLocalExitPosition(RoadPartTileState[,] roadGrid)
    {
        int gridSize = roadGrid.GetLength(0);

        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                if (roadGrid[x, z] == RoadPartTileState.Exit) return new Vector2Int(x, z);
            }
        }
        
        throw new Exception("Couldn't find entrance");
    }
    
    private List<RoadPartTileState[,]> GetAllRotatedGridParts(RoadPartTileState[,] initialGrid)
    {
        List<RoadPartTileState[,]> result = new();
        
        result.Add(initialGrid);
        result.Add(GetRotatedRoadGrid(result[^1]));
        result.Add(GetRotatedRoadGrid(result[^1]));
        result.Add(GetRotatedRoadGrid(result[^1]));

        return result;
    }

    private RoadPartTileState[,] GetRotatedRoadGrid(RoadPartTileState[,] initialGrid)
    {
        int gridSize = initialGrid.GetLength(0);
        
        RoadPartTileState[,] newGrid = new RoadPartTileState[gridSize, gridSize];

        for (int x = gridSize - 1 ; x >= 0; --x)
        {
            for (int y = 0; y < gridSize; ++y)
            {
                newGrid[y, gridSize - 1 - x] = initialGrid[x, y];
            }
        }
        
        return newGrid;
    }

    #endregion
}