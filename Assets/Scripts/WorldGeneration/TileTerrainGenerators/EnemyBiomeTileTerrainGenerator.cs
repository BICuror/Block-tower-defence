using System.Collections.Generic;
using UnityEngine;

public sealed class EnemyBiomeTileTerrainGenerator : TileTerrainGenerator
{
    private int _enemyBiomeLength;
    private bool[,] _enemyBiomeMap;
    
    public void GenerateTerrain(bool[,] enemyBiomeMap)
    {
        _enemyBiomeLength = (int)Mathf.Sqrt(enemyBiomeMap.Length);
        _enemyBiomeMap = enemyBiomeMap;
        
        for (int x = 0; x < _enemyBiomeLength; x++)
        {
            for (int z = 0; z < _enemyBiomeLength; z++)
            {
                if (enemyBiomeMap[x, z])
                {
                    GenerateTile(x, 1, z);
                }
            }
        }
    }
    
    protected override List<Vector2Int> GetNeighborPositions(int x, int y, int z)
    {
        List<Vector2Int> neighborPositions = new();

        foreach (var checkDirection in CheckDirections)
        {
            int xCheck = checkDirection.x;
            int zCheck = checkDirection.y;

            if (x + xCheck < _enemyBiomeLength && x + xCheck >= 0 && z + zCheck < _enemyBiomeLength && z + zCheck >= 0)
            {
                if (_enemyBiomeMap[x + xCheck, z + zCheck])
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
}
