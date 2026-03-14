using System.Collections.Generic;
using WorldGeneration;
using System.Linq;
using UnityEngine;
using Navigation;
using Zenject;
using Combat;

public sealed class TotemTaskGeneration : OptionalTaskGenerator
{
    [Inject] private GlobalBuildingContainer _globalBuildingContainer;
    [Inject] private IslandHeightMapHolder _islandHeightMapHolder;
    [Inject] private RoadWeightMapHolder _roadWeightMapHolder;
    [Inject] private EnemyBiomeContainer _enemyBiomeContainer;
    [Inject] private RoadMapHolder _roadMapHolder;
    [Inject] private DiContainer _diContainer;
    
    [SerializeField] private LayerSetting _terrainLayerSetting;
    [SerializeField] private LayerSetting _solidLayerSetting;
    [SerializeField] private List<Totem> _totemPrefabs;
    [SerializeField] private int _bloodTowerRadius;

    [Header("SpawnSettings")] 
    [SerializeField] private float _minimalCenterDistance = 5f;
    [SerializeField] private int _minRoadTilesInRadius = 6;
    [SerializeField] private int _minimalAverageRoadWeight = 20;
    [SerializeField] private int _maximalAverageRoadWeight = 35;
    [SerializeField] private int _maximalRadius = 10;
    private Vector2Int _centerPosition;
    
    private bool[,] _roadMap => _roadMapHolder.Map;
    
    public override bool TryGenerateOptionalTask(Vector2Int spawnerPosition, out AdditionalTaskLayerPrebuildData layerPrebuildData)
    {
        _centerPosition = new Vector2Int(_islandHeightMapHolder.Map.GetLength(0), _islandHeightMapHolder.Map.GetLength(1));
        
        layerPrebuildData = null;
        
        if (TryFindRandomPosition(spawnerPosition, out Vector2Int position))
        {
            CreateTotem(position);
            
            return true;
        }

        return false;
    }
    
    private bool TryFindRandomPosition(Vector2Int searchPosition, out Vector2Int resultPosition)
    {
        resultPosition = Vector2Int.zero;

        for (int radius = _maximalRadius; radius > 1; radius--)
        {
            List<Vector2Int> possiblePositions = TileMap.FindFurthestValidPositionsInRadius(IsAValidTotemPosition, searchPosition, radius);
            
            possiblePositions = possiblePositions.OrderBy(_ => Random.Range(0, possiblePositions.Count)).ToList();

            foreach (Vector2Int position in possiblePositions)
            {
                if (CheckTotemPositionValidity(position))
                {
                    resultPosition = position;
                    return true;
                }
            }
        }

        return false;
    }

    private bool IsAValidTotemPosition(Vector2Int position)
    {
        return TileMap.IsInBounds(position, _roadMap) && 
               !TileMap.HasTile(position, _solidLayerSetting) &&
               !_roadMap[position.x, position.y] && 
               Vector2Int.Distance(position, _centerPosition) > _minimalCenterDistance &&
               _islandHeightMapHolder.Map[position.x, position.y] > 0;
    }
    
    private bool CheckTotemPositionValidity(Vector2Int position)
    {
        List<Vector2Int> roadPositionsInRadius = TileMap.GetAllRoadPositionsInRadius(position, _roadMap, _bloodTowerRadius);

        Debug.Log($"Got {roadPositionsInRadius.Count} road positions nearby");
        
        if (roadPositionsInRadius.Count < _minRoadTilesInRadius) return false;

        float averageWeight = TileMap.GetAverageMainRoadNodeWeight(roadPositionsInRadius, _roadWeightMapHolder.Map);
            
        if (averageWeight >= _minimalAverageRoadWeight && averageWeight <= _maximalAverageRoadWeight) Debug.Log($"Calculated average weight {averageWeight}");

        return averageWeight >= _minimalAverageRoadWeight && averageWeight <= _maximalAverageRoadWeight;
    }

    private void CreateTotem(Vector2Int position)
    {
        int height = _islandHeightMapHolder.Map[position.x, position.y];
    
        if (height < 1) height = 1;
        height++;
            
        Totem randomTotemPrefab = _totemPrefabs[Random.Range(0, _totemPrefabs.Count)];
        
        Totem totem = _diContainer.InstantiatePrefab(randomTotemPrefab, new Vector3(position.x, height, position.y), Quaternion.identity, null).GetComponent<Totem>();
        _globalBuildingContainer.Add(totem.GetComponent<BuildingEntity>());
    }
}