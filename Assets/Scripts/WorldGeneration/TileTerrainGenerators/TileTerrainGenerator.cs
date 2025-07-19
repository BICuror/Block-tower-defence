using System.Collections.Generic;
using WorldGeneration;
using UnityEngine;
using System.Linq;
using ModestTree;
using Zenject;
using System;
using Random = UnityEngine.Random;

public abstract class TileTerrainGenerator : MonoBehaviour
{
    [Inject] protected IslandDataContainer IslandDataContainer;
    [Inject] private RoadMapHolder _roadMapHolder;
    [SerializeField] private Transform _tileParent;

    private List<MeshRenderer> _instantiatedTiles = new();
    
    protected Vector2Int[] CheckDirections = new Vector2Int[4]
    {
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left
    };
    
    protected void GenerateTile(int x, int y, int z)
    { 
        Vector3Int position = new Vector3Int(x, y, z);
            
        List<Vector2Int> neighborPositions = GetNeighborPositions(x, y, z);

        if (neighborPositions.Count == 4)
        {
            InstantiateTile(TileType.TopTile, position, 0f); 
        }
        else if (neighborPositions.Count == 0)
        {
            InstantiateTile(TileType.FourSideTile, position, Random.Range(0, 4) * 90f);
        }
        else if (neighborPositions.Count == 1)
        {
            InstantiateTile(TileType.ThreeSideTile, position, 90 * CheckDirections.IndexOf(neighborPositions[0]) + 90f, true);
        }
        else if (neighborPositions.Count == 2)
        {
            if (neighborPositions[0].x == neighborPositions[1].x || neighborPositions[0].y == neighborPositions[1].y)
            {
                float rotation = 0;
                
                if (neighborPositions[0].x == neighborPositions[1].x) rotation = 90;
                
                InstantiateTile(TileType.TwoSideTile, position, rotation);
            }
            else
            {
                int mainIndex = CheckDirections.IndexOf(neighborPositions[0]);
                int secondIndex = CheckDirections.IndexOf(neighborPositions[1]);

                if (mainIndex == 0 && secondIndex == 3) mainIndex = 3; 
                
                InstantiateTile(TileType.CornerTile, position, 90 * mainIndex - 90f);
            }
        }
        else
        {
            Vector2Int emptyPosition = CheckDirections.Except(neighborPositions).ToArray()[0];
            
            InstantiateTile(TileType.OneSideTile, position, 90 * CheckDirections.IndexOf(emptyPosition) + 90f, true);
        }
    }

    protected void TryGenerateFillerTile(int x, int y, int z)
    {
        List<Vector2Int> neighborPositions = GetNeighborPositions(x, y, z);

        if (neighborPositions.Count != 4)
        {
            Vector3Int position = new Vector3Int(x, y, z);
            
            InstantiateTile(TileType.DefaultTile, position, 0); 
        }
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
    
    protected abstract TilemapData GetTilemapData(int x, int z); 
    
    private void InstantiateTile(TileType type, Vector3Int position, float rotation, bool canBeMirroredByZ = false)
    {
        TilemapData tilemapData = GetTilemapData(position.x, position.z);
        
        MeshRenderer tilePrefab = GetTilePrefab(type, tilemapData);

        MeshRenderer tile = Instantiate(tilePrefab, _tileParent.position + position, Quaternion.Euler(0f, rotation, 0), _tileParent);
        
        if (canBeMirroredByZ && Random.Range(0, 100) < 50f) tile.transform.localScale = new Vector3(1f, 1f, -1f);
        
        _instantiatedTiles.Add(tile);
    }

    private MeshRenderer GetTilePrefab(TileType type, TilemapData tilemapData)
    {
        switch (type)
        {
            case TileType.DefaultTile: return tilemapData.DefaultTile;
            case TileType.FourSideTile: return tilemapData.FourSideTile;
            case TileType.ThreeSideTile: return tilemapData.ThreeSideTile;
            case TileType.TwoSideTile: return tilemapData.TwoSideTile;
            case TileType.CornerTile: return tilemapData.CornerTile;
            case TileType.OneSideTile: return tilemapData.OneSideTile;
            case TileType.TopTile: return tilemapData.TopTile;
        }
        
        throw new NotImplementedException();
    }
}

public enum TileType
{
    DefaultTile,
    FourSideTile,
    ThreeSideTile,
    TwoSideTile,
    CornerTile,
    OneSideTile,
    TopTile
}