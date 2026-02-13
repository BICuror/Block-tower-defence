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
                    BiomeData mainBiomeData = _biomeMapGenerator.GetBiomeAt(mainPosition);
                    
                    int mainHeight = _islandHeightMapHolder.Map[x, z];
                    
                    if (mainHeight == 0) continue;
                    
                    _checkDirections.ForEach(checkDirection =>
                    {
                        Vector2Int checkPosition = mainPosition + checkDirection;

                        if (IsValidPosition(checkPosition.x, checkPosition.y))
                        {
                            if (mainHeight == _islandHeightMapHolder.Map[checkPosition.x, checkPosition.y])
                            {
                                BiomeData secondaryBiomeData = _biomeMapGenerator.GetBiomeAt(checkPosition);
                                
                                if (secondaryBiomeData != mainBiomeData) TryGenerateTileSeam(mainPosition, mainBiomeData, secondaryBiomeData, checkDirection, mainHeight);
                            }
                        }
                    });
                    
                }
            }
        }

        private bool IsValidPosition(int x, int z) => (x >= 0 && z >= 0 && x < _islandData.IslandSize && z < _islandData.IslandSize); 
        
        private void TryGenerateTileSeam(Vector2Int mainPosition, BiomeData mainBiomeData, BiomeData secondaryBiomeData, Vector2Int direction, int height)
        {
            if (mainBiomeData.TilemapData.SeamPriority > secondaryBiomeData.TilemapData.SeamPriority)
            {
                float rotation = Random.Range(0, 2) * 180;
                if (direction.x != 0) rotation += 90;
                
                Vector3 spawnPosition = new Vector3(mainPosition.x + direction.x * _seamDirectionMultiplier, height + _additionalHeight, mainPosition.y + direction.y * _seamDirectionMultiplier);

                Tile tile = Instantiate(mainBiomeData.TilemapData.SeamTile, spawnPosition, Quaternion.Euler(0f, rotation, 0f), _tileParent);
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