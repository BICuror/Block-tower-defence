using System.Collections.Generic;
using WorldGeneration;
using UnityEngine;
using Zenject;

public sealed class RoadTileTerrainGenerator : TileTerrainGenerator
{
    [Inject] private IslandHeightMapHolder _heightMap;
    [Inject] private RoadMapHolder _roadMap;
    private int _islandSize;

    public void GenerateTerrain()
    {
        ClearAllTiles();

        _islandSize = IslandDataContainer.Data.IslandSize;

        for (int x = 0; x < _islandSize; x++)
        {
            for (int y = 0; y < IslandDataContainer.Data.IslandMaxHeight; y++)
            {
                for (int z = 0; z < _islandSize; z++)
                {
                    TryGenerateTile(x, y, z);
                }
            }
        }
    }

    private void TryGenerateTile(int x, int y, int z)
    {
        if (_heightMap.Map[x, z] == y && _roadMap.Map[x, z])
        {
            if (y <= 0) y = 1;
            
            GenerateTile(x, y, z);
        }
    }

    protected override List<Vector2Int> GetNeighborPositions(int x, int y, int z)
    {
        List<Vector2Int> neighborPositions = new();

        int mainHeight = _heightMap.Map[x, z];

        if (mainHeight == 0) mainHeight = 1;

        foreach (var checkDirection in CheckDirections)
        {
            int xCheck = checkDirection.x;
            int zCheck = checkDirection.y;

            if (x + xCheck < _islandSize && x + xCheck >= 0 && z + zCheck < _islandSize && z + zCheck >= 0)
            {
                int checkHeight = _heightMap.Map[x + xCheck, z + zCheck];

                if (checkHeight == 0) checkHeight = 1;

                if (checkHeight == mainHeight && _roadMap.Map[x + xCheck, z + zCheck])
                {
                    neighborPositions.Add(new Vector2Int(xCheck, zCheck));
                }
            }
        }

        return neighborPositions;
    }

    protected override bool HasTile(int x, int y, int z)
    {
        return _heightMap.Map[x, z] == y && _roadMap.Map[x, z];
    }

    protected override TilemapData GetTilemapData(int x, int z)
    {
        return IslandDataContainer.Data.RoadTilemap;
    }
}