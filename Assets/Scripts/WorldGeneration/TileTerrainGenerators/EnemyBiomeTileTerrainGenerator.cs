using System;
using System.Collections.Generic;
using WorldGeneration;
using UnityEngine;
using Zenject;

public sealed class EnemyBiomeTileTerrainGenerator : TileTerrainGenerator
{
    [Inject] private IslandHeightMapHolder _islandHeightMapHolder;
    [Inject] private IslandDataContainer _islandDataContainer;
    private int _enemyBiomeLength;
    private bool[,] _enemyBiomeMap;
    
    public void GenerateTerrain(bool[,] enemyBiomeMap)
    {
        ClearAllTiles();
        
        Vector2Int biomePosition = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
        int islandSize = _islandDataContainer.Data.IslandSize;
        
        _enemyBiomeLength = (int)Mathf.Sqrt(enemyBiomeMap.Length);
        _enemyBiomeMap = enemyBiomeMap;
        
        for (int x = 0; x < _enemyBiomeLength; x++)
        {
            for (int z = 0; z < _enemyBiomeLength; z++)
            {
                if (enemyBiomeMap[x, z])
                {
                    int xWorldPos = biomePosition.x + x;
                    int zWorldPos = biomePosition.y + z;
                    
                    GenerateTile(x, GetHeight(xWorldPos, zWorldPos), z);
                }
            }
        }
    }
    
    protected override List<Vector2Int> GetNeighborPositions(int x, int y, int z)
    {
        Vector2Int biomePosition = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
        int mainHeight = GetHeight(x + biomePosition.x, z + biomePosition.y);
        
        List<Vector2Int> neighborPositions = new();

        foreach (var checkDirection in CheckDirections)
        {
            int xCheck = checkDirection.x;
            int zCheck = checkDirection.y;
            
            int xWorldPos = xCheck + x + biomePosition.x;
            int zWorldPos = zCheck + z + biomePosition.y;
            
            int height = GetHeight(xWorldPos, zWorldPos);

            if (x + xCheck < _enemyBiomeLength && x + xCheck >= 0 && z + zCheck < _enemyBiomeLength && z + zCheck >= 0)
            {
                if (_enemyBiomeMap[x + xCheck, z + zCheck] && mainHeight <= height)
                { 
                    neighborPositions.Add(new Vector2Int(xCheck, zCheck));
                }
            }
        }

        return neighborPositions;
    }

    protected override TilemapData GetTilemapData(int x, int z)
    {
        return IslandDataContainer.Data.EnemyBiomeTilemap;
    }

    private int GetHeight(int x, int z)
    {
        int islandSize = _islandDataContainer.Data.IslandSize;
        
        if (x >= 0 && z >= 0 && x < islandSize && z < islandSize) return Math.Max(1, _islandHeightMapHolder.Map[x, z]);

        return 1;
    }
}
