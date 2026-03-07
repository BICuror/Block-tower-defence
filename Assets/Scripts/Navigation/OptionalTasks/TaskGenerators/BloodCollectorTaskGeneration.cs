using System.Collections.Generic;
using WorldGeneration;
using System.Linq;
using Combat;
using UnityEngine;
using Navigation;
using Zenject;

public sealed class BloodCollectorTaskGeneration : OptionalTaskGenerator
{
    [Inject] private GlobalBuildingContainer _globalBuildingContainer;
    [Inject] private IslandHeightMapHolder _islandHeightMapHolder;
    [Inject] private EnemyBiomeContainer _enemyBiomeContainer;
    [Inject] private RoadMapHolder _roadMapHolder;
    [Inject] private DiContainer _diContainer;
    
    [SerializeField] private LayerSetting _terrainLayerSetting;
    [SerializeField] private LayerSetting _solidLayerSetting;
    [SerializeField] private BloodCollector _bloodTowerPrefab;
    [SerializeField] private int _bloodTowerRadius;
    [SerializeField] private float _requiredPercentFromSpawner = 0.33f;

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
        _centerPosition = new Vector2Int(_islandHeightMapHolder.Map.GetLength(0), _islandHeightMapHolder.Map.GetLength(1));
        
        layerPrebuildData = null;
        
        if (TryFindRandomPosition(spawnerPosition, out Vector2Int position))
        {
            int height = _islandHeightMapHolder.Map[position.x, position.y];
    
            if (height < 1) height = 1;
            height++;
            
            BloodCollector bloodCollector = _diContainer.InstantiatePrefab(_bloodTowerPrefab, new Vector3(position.x, height, position.y), Quaternion.identity, null).GetComponent<BloodCollector>();
            _globalBuildingContainer.Add(bloodCollector.GetComponent<BuildingEntity>());

            int incomingEnemiesAmount = _enemyBiomeContainer.EnemyBiomeList.First(biome => biome.GetCenterPosition() == spawnerPosition).EnemySpawner.EntitiesAmountToSpawn;
            incomingEnemiesAmount = Mathf.RoundToInt(incomingEnemiesAmount * _requiredPercentFromSpawner);
            
            if (incomingEnemiesAmount < 1) incomingEnemiesAmount = 1;
            
            bloodCollector.SetRequiredKills(incomingEnemiesAmount);
            
            return true;
        }

        return false;
    }
    
    private bool TryFindRandomPosition(Vector2Int searchPosition, out Vector2Int resultPosition)
    {
        resultPosition = Vector2Int.zero;
        
        for (int radius = _maximalRadius; radius > 1; radius--)
        {
            List<Vector2Int> possiblePositions = TileMap.FindClosestValidPositionsPerRadius(IsNotARoadTile, searchPosition, radius, radius);

            possiblePositions = possiblePositions.OrderBy(_ => Random.Range(0, possiblePositions.Count)).ToList();
            
            foreach (Vector2Int position in possiblePositions)
            {
                if (CheckBloodCollectorPositionValidity(position, searchPosition))
                {   
                    resultPosition = position;
                    return true;
                }
            }         
        }
        
        return false;
        
        
        bool IsNotARoadTile(Vector2Int position) => IsValidPosition(position.x, position.y) && !_roadMap[position.x, position.y] && !TileMap.HasTile(position, _solidLayerSetting);
    }
    
    private bool CheckBloodCollectorPositionValidity(Vector2Int position, Vector2Int spawnerPosition)
    {
        if (Vector2Int.Distance(position, _centerPosition) < _minimalCenterDistance) return false;
        
        if (!TileMap.HasTile(position, _terrainLayerSetting)) return false;
         
        List<Vector2Int> possiblePositions = TileMap.FindClosestValidPositionsPerRadius(IsARoadTile, position, _bloodTowerRadius, _bloodTowerRadius);

        if (possiblePositions.Count >= _minRoadTilesInRadius && possiblePositions.Count <= _maxRoadTilesInRadius)
        {
            List<int> roadWeights = new List<int>();
            
            possiblePositions.ForEach(roadPosition =>
            {
                TileMap.HasAValidRoadFromStartToEnd(_roadMap, roadPosition, spawnerPosition, int.MinValue, int.MaxValue, out int roadWeight);
                
                roadWeights.Add(roadWeight);
            });

            float averageWeight = (float)roadWeights.Average();
            
            return averageWeight >= _minimalAverageRoadWeight && averageWeight <= _maximalAverageRoadWeight;
        }

        return false;
        
        bool IsARoadTile(Vector2Int position) => IsValidPosition(position.x, position.y) && _roadMap[position.x, position.y];
    }
    
    private bool IsValidPosition(int x, int y) => x >= 0 && x < _roadMap.GetLength(0) && y >= 0 && y < _roadMap.GetLength(1);
}
