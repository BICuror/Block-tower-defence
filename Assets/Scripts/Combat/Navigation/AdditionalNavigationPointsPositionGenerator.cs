using System.Collections.Generic;
using UnityEngine;
using Zenject;
using WorldGeneration;
using System.Linq;

public sealed class AdditionalNavigationPointsPositionGenerator : MonoBehaviour
{
     private readonly Vector2Int[] _checkDirections = new Vector2Int[4]
     {
         Vector2Int.up,
         Vector2Int.down,
         Vector2Int.right,
         Vector2Int.left
     };
     
    [Inject] private RoadMapHolder _roadMapHolder;
    [SerializeField] private LayerSetting _solidObjectsLayer;
    [SerializeField] private int _initialRaduis;

    private bool[,] _roadMap => _roadMapHolder.Map;
    private int _currentMapCenter;
    private int _currentMapSize;

    public List<Vector2Int> GeneratePositionsAndConnectToRoad(List<Vector2Int> spawnerPositions)
    {
        _currentMapSize = _roadMap.GetLength(0);
        _currentMapCenter = (_roadMap.GetLength(0) - 1) / 2;

        List<Vector2Int> navigationPointsPositions = new();

        foreach (Vector2Int spawnerPosition in spawnerPositions)
        {
            if (TryFindRandomPosition(spawnerPosition, _initialRaduis, out Vector2Int selectedPosition))
            {
                navigationPointsPositions.Add(selectedPosition);
            }
        }

        return navigationPointsPositions;
    }
    
    private bool[,] _positionCheckMap;

    private bool TryFindRandomPosition(Vector2Int centerPosition, int startingRadius, out Vector2Int selectedPoisition)
    {
        for (int radius = startingRadius; radius > 1; radius--)
        {
            List<Vector2Int> possiblePositions = FindAllSutablePositions(centerPosition, radius);

            possiblePositions = possiblePositions.OrderBy(_ => Random.Range(0, possiblePositions.Count)).ToList();
            
            foreach (Vector2Int position in possiblePositions)
            {
                _positionCheckMap = new bool[_currentMapSize, _currentMapSize];
                List<Vector2Int> initialPositions = new();
                initialPositions.Add(position);
                Vector2Int roadTile = FindClosestRoadTile(initialPositions);

                float distance = Mathf.Abs(position.x - roadTile.x) + Mathf.Abs(position.y - roadTile.y);

                if (HasRoadToPosition(roadTile, centerPosition) && distance > 2 && distance <= 5)
                {   
                    ConnectPositionsOnRoadMap(position, roadTile);

                    selectedPoisition = position;
                    return true;
                }
            }         
        }
    
        Debug.LogError("Couldn't find suitable random position");            
        selectedPoisition = new Vector2Int(0, 0);
        
        return false;
    }

    private bool HasSolidObjects(int x, int y)
    {
        Ray heightRay = new Ray(new Vector3(x, 100000f, y), Vector3.down);

        return Physics.Raycast(heightRay, Mathf.Infinity, _solidObjectsLayer.GetLayerMask());
    }
    
    private Vector2Int FindClosestRoadTile(List<Vector2Int> initialPositions)
    {
        List<Vector2Int> positionsToCheck = new();
        
        foreach (Vector2Int currentPosition in initialPositions)
        {
            _positionCheckMap[currentPosition.x, currentPosition.y] = true;

            foreach (Vector2Int checkDirection in _checkDirections)
            {
                int x = currentPosition.x + checkDirection.x;
                int y = currentPosition.y + checkDirection.y;

                if (IsValidPosition(x, y) == false || _positionCheckMap[x, y]) continue;
        
                if (_roadMap[x, y]) return new Vector2Int(x, y);

                positionsToCheck.Add(new Vector2Int(x, y));
            }
        }

        if (positionsToCheck.Count > 0) return FindClosestRoadTile(positionsToCheck);
        
        Debug.LogError("Couldn't find any road tiles nearby");
        return Vector2Int.zero;    
    }

    private void ConnectPositionsOnRoadMap(Vector2Int startPosition, Vector2Int endPosition)
    {
        bool[,] roadMap = _roadMapHolder.Map;

        Vector2Int currentPosition = startPosition;
        roadMap[currentPosition.x, currentPosition.y] = true;
        int iterations = 0;
        while (currentPosition != endPosition && iterations < 1000)
        {
            iterations++;
            Vector2Int direction = endPosition - currentPosition;

            int x = direction.x;
            int y = direction.y;

            int normalizedX = 0;
            if (x < 0) normalizedX = -1;
            else if (x > 0) normalizedX = 1;

            int normalizedY = 0;
            if (y < 0) normalizedY = -1;
            else if (y > 0) normalizedY = 1;

            if (normalizedX == 0) currentPosition = currentPosition + new Vector2Int(0, normalizedY);
            else if (normalizedY == 0) currentPosition = currentPosition + new Vector2Int(normalizedX, 0);
            else if (normalizedX != 0 && normalizedY != 0)
            {
                if (Random.Range(0, 100) > 50) currentPosition = currentPosition + new Vector2Int(0, normalizedY);
                else currentPosition = currentPosition + new Vector2Int(normalizedX, 0);
            }   

            roadMap[currentPosition.x, currentPosition.y] = true;
        }

        _roadMapHolder.SetRoadMap(roadMap);
    }

    private List<Vector2Int> FindAllSutablePositions(Vector2Int centerPosition, int radius)
    {
        List<Vector2Int> possiblePositions = new();
        
        for (int x = -radius; x < radius; x++)
        {
            for (int y = -radius; y < radius; y++)
            {                        
                if ((x == -radius || x == radius - 1) || (y == -radius || y == radius - 1))
                {
                    int checkX = x + centerPosition.x;
                    int checkY = y + centerPosition.y;
                        
                    if (IsValidPosition(checkX, checkY) == false || _roadMap[checkX, checkY]) continue;
                     
                    if (HasSolidObjects(checkX, checkY)) continue;

                    bool hasRoadAround = false;

                    foreach (Vector2Int checkDirection in _checkDirections)
                    {
                        int roadCheckX = checkDirection.x + checkX;
                        int roadCheckY = checkDirection.y + checkY;

                        hasRoadAround = IsValidPosition(roadCheckX, roadCheckY) == false || _roadMap[roadCheckX, roadCheckY];
                    }

                    if (hasRoadAround == false)
                    {
                        possiblePositions.Add(new Vector2Int(checkX, checkY));   
                    }
                }
            }
        } 

        return possiblePositions;
    }
    
    private bool[,] _searchMap;

    private bool HasRoadToPosition(Vector2Int currentPosition, Vector2Int endPosition)
    {
        _searchMap = new bool[_currentMapSize, _currentMapSize];

        return TryToFindRoadToSpawner(currentPosition, endPosition);
    }
    
    private bool TryToFindRoadToSpawner(Vector2Int currentPosition, Vector2Int endPosition)
    {
        if (currentPosition == endPosition) return true;

        int x = currentPosition.x;
        int y = currentPosition.y;
        
        if (_searchMap[x, y] == true) return false;
        
        _searchMap[x, y] = true;
        
        if (Vector2Int.Distance(currentPosition, new Vector2Int(_currentMapCenter, _currentMapCenter)) < 3f) return false;
            
        int xDir = 1;
        if (currentPosition.x < _currentMapCenter) xDir = -1;
        int yDir = 1;
        if (currentPosition.y < _currentMapCenter) yDir = -1;

        if (IsValidForSpawnSearch(x + xDir, y) && TryToFindRoadToSpawner(currentPosition + new Vector2Int(xDir, 0), endPosition)) return true;
        if (IsValidForSpawnSearch(x, y + yDir) && TryToFindRoadToSpawner(currentPosition + new Vector2Int(0, yDir), endPosition)) return true;
        if (IsValidForSpawnSearch(x - xDir, y) && TryToFindRoadToSpawner(currentPosition + new Vector2Int(-xDir, 0), endPosition)) return true;
        if (IsValidForSpawnSearch(x, y - yDir) && TryToFindRoadToSpawner(currentPosition + new Vector2Int(0, -yDir), endPosition)) return true;
            
        return false;
    }

    private bool IsValidForSpawnSearch(int x, int y) => IsValidPosition(x, y) && _roadMap[x, y];

    private bool IsValidPosition(int x, int y) => x >= 0 && x < _currentMapSize && y >= 0 && y < _currentMapSize;
}