using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using ModestTree;
using Zenject;
using System;

using Random = UnityEngine.Random;

public abstract class TileTerrainGenerator : MonoBehaviour
{
    [Inject] protected IslandDataContainer IslandDataContainer;
    [SerializeField] private Transform _tileParent;

    private List<GPUInstanceEnabler> _instantiatedTiles = new();
    
    protected Vector2Int[] CheckDirections = new Vector2Int[4]
    {
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left
    };
    
    protected Vector2Int[] CornerCheckDirections = new Vector2Int[4]
    {
        new Vector2Int(-1, 1),
        new Vector2Int(1, 1),
        new Vector2Int(1, -1),
        new Vector2Int(-1, -1),
    };

    public List<GPUInstanceEnabler> InstantiatedTiles => _instantiatedTiles;
    
    protected void GenerateTile(int x, int y, int z)
    { 
        Vector3 position = new Vector3(x, y, z);
            
        List<Vector2Int> tileNeighborPositions = GetNeighborPositions(x, y, z);
        
        for (int xOffset = -1; xOffset <= 1; xOffset += 2)
        {
            for (int zOffset = -1; zOffset <= 1; zOffset += 2)
            {
                List<Vector2Int> neighborPositions = new();
                
                if (tileNeighborPositions.Exists(position => position.x == xOffset)) neighborPositions.Add(new Vector2Int(xOffset, 0));
                neighborPositions.Add(new Vector2Int(-xOffset, 0));
                
                if (tileNeighborPositions.Exists(position => position.y == zOffset)) neighborPositions.Add(new Vector2Int(0, zOffset));
                neighborPositions.Add(new Vector2Int(0, -zOffset));

                Vector3 spawnPosition = position;
                
                spawnPosition.x += xOffset * 0.25f;
                spawnPosition.z += zOffset * 0.25f;
                
                if (neighborPositions.Count == 4)
                {
                    if (HasTile(x + xOffset, y, z + zOffset))
                    {
                        InstantiateTile(TileType.DefaultTile, spawnPosition, 0f);
                    }
                    else
                    {
                        int index = CornerCheckDirections.IndexOf(new Vector2Int(xOffset, zOffset));
                        
                        InstantiateTile(TileType.DefaultCornerTile, spawnPosition, index * 90f);
                    }
                }
                else if (neighborPositions.Count == 2)
                {
                    neighborPositions = neighborPositions.OrderBy(position => CheckDirections.IndexOf(position)).ToList();
                    
                    int mainIndex = CheckDirections.IndexOf(neighborPositions[0]);
                    int secondIndex = CheckDirections.IndexOf(neighborPositions[1]);
    
                    if (mainIndex == 0 && secondIndex == 3) mainIndex = 3; 
                    
                    InstantiateTile(TileType.CornerTile, spawnPosition, 90 * mainIndex - 90f);
                }
                else
                {
                    Vector2Int emptyPosition = CheckDirections.Except(neighborPositions).ToArray()[0];
                    
                    InstantiateTile(TileType.OneSideTile, spawnPosition, 90 * CheckDirections.IndexOf(emptyPosition) + 90f, true);
                }
            }
        }
    }

    protected void TryGenerateFillerTile(int x, int y, int z)
    {
        GenerateTile(x, y, z);
    }

    protected void ClearAllTiles()
    {
        for (int i = 0; i < _instantiatedTiles.Count; i++)
        {
            Destroy(_instantiatedTiles[i].gameObject);
        }
        
        _instantiatedTiles.Clear();
    }
    
    protected abstract List<Vector2Int> GetNeighborPositions(int x, int y, int z);

    protected abstract bool HasTile(int x, int y, int z);
    
    protected abstract TilemapData GetTilemapData(int x, int z); 
    
    private void InstantiateTile(TileType type, Vector3 tilePosition, float rotation, bool canBeMirroredByZ = false)
    {
        Vector3Int position = new Vector3Int(Mathf.RoundToInt(tilePosition.x), Mathf.RoundToInt(tilePosition.y), Mathf.RoundToInt(tilePosition.z));
        
        TilemapData tilemapData = GetTilemapData(position.x, position.z);
        
        GPUInstanceEnabler tilePrefab = GetTilePrefab(type, tilemapData);

        GPUInstanceEnabler tile = Instantiate(tilePrefab, _tileParent.position + tilePosition, Quaternion.Euler(0f, rotation, 0), _tileParent);
        
        if (canBeMirroredByZ && Random.Range(0, 100) < 50f) tile.transform.localScale = new Vector3(0.5f, 1f, -0.5f);
        else tile.transform.localScale = new Vector3(0.5f, 1f, 0.5f);
        
        _instantiatedTiles.Add(tile);
    }

    private GPUInstanceEnabler GetTilePrefab(TileType type, TilemapData tilemapData)
    {
        switch (type)
        {
            case TileType.DefaultTile: return tilemapData.DefaultTile;
            case TileType.CornerTile: return tilemapData.CornerTile;
            case TileType.OneSideTile: return tilemapData.OneSideTile;
            case TileType.DefaultCornerTile: return tilemapData.DefaultCornerTile;
        }
        
        throw new NotImplementedException();
    }
}

public enum TileType
{
    DefaultTile,
    DefaultCornerTile,
    CornerTile,
    OneSideTile,
}