using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace WorldGeneration
{
    public sealed class TilemapOverlapSeamsGenerator : MonoBehaviour
    {
        [Inject] private IslandHeightMapHolder _islandHeightMapHolder;
        [Inject] private IslandDataContainer _islandDataContainer;
        [Inject] private BiomeMapGenerator _biomeMapGenerator;

        [SerializeField] private float _seamDirectionMultiplier = 0.5f;
        [SerializeField] private float _additionalHeight = 0.501f;
        [SerializeField] private Transform _tileParent;
        
        private List<Tile> _instantiatedTiles = new();
        
        private List<Vector2Int> _checkDirections = new List<Vector2Int>
        {
            Vector2Int.down,
            Vector2Int.up,
            Vector2Int.right,
            Vector2Int.left
        };
        
        private IslandData _islandData => _islandDataContainer.Data;
        
        public void GenerateTerrain()
        {
            ClearTiles();
            
            int islandSize = _islandData.IslandSize;

            for (int x = 0; x < islandSize; x++)
            {
                for (int z = 0; z < islandSize; z++)
                {
                    Vector2Int mainPosition = new Vector2Int(x, z);
                    
                    int mainHeight = _islandHeightMapHolder.Map[x, z];
                    
                    if (mainHeight == 0) continue;
                    
                    _checkDirections.ForEach(checkDirection =>
                    {
                        Vector2Int checkPosition = mainPosition + checkDirection;

                        if (IsValidPosition(checkPosition.x, checkPosition.y))
                        {
                            if (mainHeight == _islandHeightMapHolder.Map[checkPosition.x, checkPosition.y])
                            {
                                 TryGenerateTileSeam(mainPosition, checkDirection);
                            }
                        }
                    });
                }
            }
        }

        private bool IsValidPosition(int x, int z) => (x >= 0 && z >= 0 && x < _islandData.IslandSize && z < _islandData.IslandSize); 
        
        private void TryGenerateTileSeam(Vector2Int mainPosition, Vector2Int direction)
        {
            BiomeData mainBiomeData = _biomeMapGenerator.GetBiomeAt(mainPosition);
            BiomeData secondaryBiomeData = _biomeMapGenerator.GetBiomeAt(mainPosition + direction);
            
            if (secondaryBiomeData == mainBiomeData) return;
            
            if (mainBiomeData.TilemapData.SeamPriority > secondaryBiomeData.TilemapData.SeamPriority)
            {
                float rotation = Random.Range(0, 2) * 180;
                if (direction.x != 0) rotation += 90;

                float height = _islandHeightMapHolder.Map[mainPosition.x, mainPosition.y] + _additionalHeight;

                Vector2 spawnPosition = mainPosition + (Vector2)direction * _seamDirectionMultiplier;

                Tile tile = Instantiate(mainBiomeData.TilemapData.SeamTile, new Vector3(spawnPosition.x, height, spawnPosition.y), Quaternion.Euler(0f, rotation, 0f), _tileParent);
                
                _instantiatedTiles.Add(tile);
            }
        }

        private void ClearTiles()
        {
            _instantiatedTiles.ForEach(tile => Destroy(tile.gameObject));
            _instantiatedTiles.Clear();
        }
    }
}