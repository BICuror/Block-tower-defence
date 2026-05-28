using System.Collections.Generic;
using WorldGeneration;
using UnityEngine;
using System.Linq;
using Navigation;
using Combat;
using Zenject;

public sealed class ChestOptionalTaskGenerator : OptionalTaskGenerator
{
    
    private readonly Vector2Int[] _checkDirections = new Vector2Int[4]
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.right,
        Vector2Int.left
    };

    [Inject] private GlobalBuildingContainer _globalBuildingContainer;
    [Inject] private RoadWeightMapGenerator _roadWeightMapGenerator;
    [Inject] private IslandHeightMapHolder _islandHeightMapHolder;
    [Inject] private RoadWeightMapHolder _roadWeightMapHolder;
    [Inject] private RoadMapHolder _roadMapHolder;
    [Inject] private DiContainer _diContainer;

    [SerializeField] private Chest _chestPrefab;
    
    [Header("WeightSettings")]
    [SerializeField] private int _minimalWeightFromSpawner = 20;
    [SerializeField] private int _maximalWeightFromSpawner = 40;

    [Header("AdditionalRoad")] 
    [SerializeField] private int _minimalAdditionalRoad = 1;
    [SerializeField] private int _maximalAdditionalRoad = 5;
    
    [Header("SpawnSettings")]
    [SerializeField] private int _maximalRaduis = 12;

    private bool[,] _roadMap => _roadMapHolder.Map;
    private int _currentMapSize;
    private int _centerIndex;

    public override bool TryGenerateOptionalTask(Vector2Int spawnerPosition, out AdditionalTaskLayerPrebuildData layerPrebuildData)
    {
        layerPrebuildData = null;
        
        _currentMapSize = _roadMap.GetLength(0);
        _centerIndex = _currentMapSize / 2 + 1;

        if (TryFindRandomPosition(spawnerPosition, out Vector2Int resultPosition))
        {
            int height = _islandHeightMapHolder.Map[resultPosition.x, resultPosition.y];
    
            if (height < 1) height = 1;
            height++;
    
            GameObject chest = _diContainer.InstantiatePrefab(_chestPrefab, new Vector3(resultPosition.x, height, resultPosition.y), Quaternion.identity, null);
            
            _globalBuildingContainer.Add(chest.GetComponent<BuildingEntity>());
            
            layerPrebuildData = new AdditionalTaskLayerPrebuildData(new ExsistanceNavigationCondition(chest), resultPosition);
        
            return true;
        }
        
        return false;
    }

    private bool TryFindRandomPosition(Vector2Int searchPosition, out Vector2Int selectedPosition)
    {
        selectedPosition = new Vector2Int(0, 0);
        
        for (int radius = _maximalRaduis; radius > 1; radius--)
        {
            List<Vector2Int> possiblePositions = FindAllSuitablePositions(searchPosition, radius);

            possiblePositions = possiblePositions.OrderBy(_ => Random.Range(0, possiblePositions.Count)).ToList();
            
            foreach (Vector2Int position in possiblePositions)
            {
                if (Vector2.Distance(new Vector2(_centerIndex, _centerIndex), searchPosition) > Vector2.Distance(position, searchPosition))
                {
                    if (TryFindClosestValidRoadTile(position, out Vector2Int roadTile))
                    {
                        int roadWeight = _roadWeightMapHolder.Map[roadTile.x, roadTile.y];
    
                        if (roadWeight >= _minimalWeightFromSpawner && roadWeight <= _maximalWeightFromSpawner)
                        {
                            ConnectPositionsOnRoadMap(position, roadTile);
        
                            selectedPosition = position;
                            return true;
                        }
                    }
                }
            }         
        }
        
        return false;
    }
    
    private bool TryFindClosestValidRoadTile(Vector2Int searchPosition, out Vector2Int roadTilePosition)
    {
        roadTilePosition = Vector2Int.zero;
        
        List<Vector2Int> validPositions = TileMap.FindClosestValidPositionsInRadius(RoadPositionValidator, searchPosition, 0, _currentMapSize / 2);
            
        if (validPositions.Count > 0) roadTilePosition = validPositions[Random.Range(0, validPositions.Count)];
        
        return validPositions.Count > 0;
        
        bool RoadPositionValidator(Vector2Int position)
        {
            if (IsValidPosition(position.x, position.y) && _roadMap[position.x, position.y])
            {
                float distanceToRoad = Mathf.Abs(searchPosition.x - position.x) + Mathf.Abs(searchPosition.y - position.y);

                return distanceToRoad >= _minimalAdditionalRoad && distanceToRoad <= _maximalAdditionalRoad;
            }

            return false;
        }
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

            if (normalizedX == 0) currentPosition += new Vector2Int(0, normalizedY);
            else if (normalizedY == 0) currentPosition += new Vector2Int(normalizedX, 0);
            else if (normalizedX != 0 && normalizedY != 0)
            {
                if (Random.Range(0, 100) > 50) currentPosition = currentPosition + new Vector2Int(0, normalizedY);
                else currentPosition = currentPosition + new Vector2Int(normalizedX, 0);
            }   

            roadMap[currentPosition.x, currentPosition.y] = true;
        }

        _roadMapHolder.SetRoadMap(roadMap);
        _roadWeightMapGenerator.GenerateRoadWeightMap();
    }

    private List<Vector2Int> FindAllSuitablePositions(Vector2Int centerPosition, int radius)
    {
        List<Vector2Int> possiblePositions = new();

        possiblePositions = TileMap.FindClosestValidPositionsInRadius(ValidatePosition, centerPosition, radius, _currentMapSize / 2);

        return possiblePositions;

        bool ValidatePosition(Vector2Int position)
        {
            if (IsValidPosition(position.x, position.y) == false || _roadMap[position.x, position.y]) return false;
                     
            if (TileMap.HasTile(position, LayerSettingType.SolidObjects)) return false;

            bool hasRoadAround = false;

            foreach (Vector2Int checkDirection in _checkDirections)
            {
                int roadCheckX = checkDirection.x + position.x;
                int roadCheckY = checkDirection.y + position.y;

                if (!IsValidPosition(roadCheckX, roadCheckY) || _roadMap[roadCheckX, roadCheckY]) hasRoadAround = true;
            }
            
            return !hasRoadAround;
        }
    }
    
    private bool IsValidPosition(int x, int y) => x >= 0 && x < _currentMapSize && y >= 0 && y < _currentMapSize;
}