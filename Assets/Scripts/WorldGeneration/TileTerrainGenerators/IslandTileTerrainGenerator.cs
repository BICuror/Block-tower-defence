using System.Collections.Generic;
using WorldGeneration;
using UnityEngine;
using Zenject;

public sealed class IslandTileTerrainGenerator : TileTerrainGenerator
{
    [Inject] private BiomeMapGenerator _biomeMapGenerator;
    [Inject] private IslandHeightMapHolder _heightMap;
    private int _islandSize;

    public void GenerateTerrain()
    {
        ClearAllTiles();
        
        _islandSize = IslandDataContainer.Data.IslandSize;

        for (int x = 0; x < _islandSize; x++)
        {
            for (int y = 1; y < IslandDataContainer.Data.IslandMaxHeight; y++)
            {
                for (int z = 0; z < _islandSize; z++)
                {
                    TryToCreateTile(x, y, z);
                }
            }
        }
    }

    private void TryToCreateTile(int x, int y, int z)
    {
        if (_heightMap.Map[x, z] == y)
        {
            GenerateTile(x, y, z);
        }
        else if (_heightMap.Map[x, z] > y)
        {
            TryGenerateFillerTile(x, y, z);
        }
    }

    protected override List<Vector2Int> GetNeighborPositions(int x, int y, int z)
    {
        List<Vector2Int> neighborPositions = new();
        
        foreach (var checkDirection in CheckDirections)
        {
            int xCheck = checkDirection.x;
            int zCheck = checkDirection.y;
           
            if (x + xCheck < _islandSize && x + xCheck >= 0 && z + zCheck < _islandSize && z + zCheck >= 0) 
            { 
                int checkHeight = _heightMap.Map[x + xCheck, z + zCheck];
                
                if (checkHeight >= y) 
                { 
                    neighborPositions.Add(new Vector2Int(xCheck, zCheck));
                }
            }
        }     
        
        return neighborPositions;
    }

    protected override bool HasTile(int x, int y, int z)
    {
        return _heightMap.Map[x, z] >= y;
    }

    protected override TilemapData GetTilemapData(int x, int z)
    {
        return _biomeMapGenerator.GetBiomeAt(new Vector2Int(x, z)).TilemapData;
    }
}