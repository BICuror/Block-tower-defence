using System.Collections.Generic;
using WorldGeneration;
using System.Linq;
using UnityEngine;
using Navigation;
using Zenject;
using Combat;

public sealed class ChargableTotemTaskGeneration : OptionalTaskGenerator
{
    [Inject] private GlobalBuildingContainer _globalBuildingContainer;
    [Inject] private IslandHeightMapHolder _islandHeightMapHolder;
    [Inject] private RoadWeightMapHolder _roadWeightMapHolder;
    [Inject] private EnemyBiomeContainer _enemyBiomeContainer;
    [Inject] private RoadMapHolder _roadMapHolder;
    [Inject] private DiContainer _diContainer;
    
    [SerializeField] private LayerSetting _terrainLayerSetting;
    [SerializeField] private LayerSetting _solidLayerSetting;
    [SerializeField] private ChargableTotem _chargableTotemPrefab;
    [SerializeField] private int _chargeTotemRadius;

    [Header("SpawnSettings")] 
    [SerializeField]private float _minimalCenterDistance = 5f;
    [SerializeField] private int _maxRoadTilesInRadius = 15;
    [SerializeField] private int _minRoadTilesInRadius = 6;
    [SerializeField] private int _minimalAverageRoadWeight = 20;
    [SerializeField] private int _maximalAverageRoadWeight = 35;
    [SerializeField] private int _maximalRadius = 10;
    private Vector2Int _centerPosition;
    
    private bool[,] _roadMap => _roadMapHolder.Map;
    
    public override bool TryGenerateOptionalTask(Vector2Int spawnerPosition, out AdditionalTaskLayerPrebuildData layerPrebuildData)
    {
        layerPrebuildData = null;
        
        if (_enemyBiomeContainer.EnemyBiomeList.First(biome => biome.GetCenterPosition() == spawnerPosition).EnemySpawner.EnemiesToSpawn.Exists(enemyData => enemyData.DiesOnContact))
        {
            return false;
        }
        
        _centerPosition = new Vector2Int(_islandHeightMapHolder.Map.GetLength(0), _islandHeightMapHolder.Map.GetLength(1));
        
        if (TryFindRandomPosition(spawnerPosition, out Vector2Int position))
        {
            CreateChargableTotem(position, spawnerPosition);
            
            return true;
        }

        return false;
    }
    
    private bool TryFindRandomPosition(Vector2Int searchPosition, out Vector2Int resultPosition)
    {
        resultPosition = Vector2Int.zero;

        for (int radius = _maximalRadius; radius > 1; radius--)
        {
            List<Vector2Int> possiblePositions = TileMap.FindFurthestValidPositionsInRadius(IsAChargableTotemPosition, searchPosition, radius);
            
            possiblePositions = possiblePositions.OrderBy(_ => Random.Range(0, possiblePositions.Count)).ToList();

            foreach (Vector2Int position in possiblePositions)
            {
                if (CheckChargableTotemPositionValidity(position))
                {
                    resultPosition = position;
                    return true;
                }
            }
        }

        return false;
    }

    private bool IsAChargableTotemPosition(Vector2Int position)
    {
        return TileMap.IsInBounds(position, _roadMap) && 
               !TileMap.HasTile(position, _solidLayerSetting) &&
               !_roadMap[position.x, position.y] && 
               Vector2Int.Distance(position, _centerPosition) > _minimalCenterDistance &&
               _islandHeightMapHolder.Map[position.x, position.y] > 0;
    }
    
    private bool CheckChargableTotemPositionValidity(Vector2Int position)
    {
        List<Vector2Int> roadPositionsInRadius = TileMap.GetAllRoadPositionsInRadius(position, _roadMap, _chargeTotemRadius);

        Debug.Log($"Got {roadPositionsInRadius.Count} road positions nearby");
        
        if (roadPositionsInRadius.Count < _minRoadTilesInRadius || roadPositionsInRadius.Count > _maxRoadTilesInRadius) return false;

        float averageWeight = TileMap.GetAverageMainRoadNodeWeight(roadPositionsInRadius, _roadWeightMapHolder.Map);
            
        if (averageWeight >= _minimalAverageRoadWeight && averageWeight <= _maximalAverageRoadWeight) Debug.Log($"Calculated average weight {averageWeight}");

        return averageWeight >= _minimalAverageRoadWeight && averageWeight <= _maximalAverageRoadWeight;
    }

    private void CreateChargableTotem(Vector2Int position, Vector2Int spawnerPosition)
    {
        int height = _islandHeightMapHolder.Map[position.x, position.y];
    
        if (height < 1) height = 1;
        height++;
            
        ChargableTotem chargeableTotem = _diContainer.InstantiatePrefab(_chargableTotemPrefab, new Vector3(position.x, height, position.y), Quaternion.identity, null).GetComponent<ChargableTotem>();
        _globalBuildingContainer.Add(chargeableTotem.GetComponent<BuildingEntity>());

        int incomingEnemiesAmount = _enemyBiomeContainer.EnemyBiomeList.First(biome => biome.GetCenterPosition() == spawnerPosition).EnemySpawner.EntitiesAmountToSpawn;
        incomingEnemiesAmount = Mathf.RoundToInt(incomingEnemiesAmount);
            
        if (incomingEnemiesAmount < 1) incomingEnemiesAmount = 1;
            
        chargeableTotem.CalculateRequiredCharge(incomingEnemiesAmount, TileMap.GetAllRoadPositionsInRadius(position, _roadMap, _chargeTotemRadius).Count);
    }
}